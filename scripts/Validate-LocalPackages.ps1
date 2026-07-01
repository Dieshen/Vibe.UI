[CmdletBinding()]
param(
    [string]$PackagesPath = "packages",
    [string]$Version,
    [switch]$KeepTemp
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Resolve-PathFromRepo {
    param([Parameter(Mandatory = $true)][string]$Path)

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
}

function Invoke-Checked {
    param(
        [Parameter(Mandatory = $true)][string]$FilePath,
        [string[]]$ArgumentList = @()
    )

    Write-Host "> $FilePath $($ArgumentList -join ' ')"
    & $FilePath @ArgumentList
    if ($LASTEXITCODE -ne 0) {
        throw "Command failed with exit code ${LASTEXITCODE}: $FilePath $($ArgumentList -join ' ')"
    }
}

function Get-ProjectVersion {
    $projectPath = Join-Path $repoRoot "src/Vibe.UI/Vibe.UI.csproj"
    [xml]$project = Get-Content -LiteralPath $projectPath
    $versionNode = $project.Project.PropertyGroup | Where-Object { $_.Version } | Select-Object -First 1

    if ($null -eq $versionNode -or [string]::IsNullOrWhiteSpace($versionNode.Version)) {
        throw "Could not find <Version> in $projectPath."
    }

    return $versionNode.Version
}

function Assert-FileExists {
    param([Parameter(Mandatory = $true)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw "Expected file was not found: $Path"
    }
}

function Assert-PackageEntry {
    param(
        [Parameter(Mandatory = $true)][string]$PackagePath,
        [Parameter(Mandatory = $true)][string]$EntryName
    )

    $zip = [System.IO.Compression.ZipFile]::OpenRead($PackagePath)
    try {
        $entry = $zip.Entries | Where-Object { $_.FullName -eq $EntryName } | Select-Object -First 1
        if ($null -eq $entry) {
            throw "Package $PackagePath is missing entry $EntryName."
        }
    }
    finally {
        $zip.Dispose()
    }
}

function Write-NuGetConfig {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$LocalSource,
        [Parameter(Mandatory = $true)][string]$GlobalPackagesFolder
    )

    $escapedSource = [System.Security.SecurityElement]::Escape($LocalSource)
    $escapedGlobalPackages = [System.Security.SecurityElement]::Escape($GlobalPackagesFolder)

    $content = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <config>
    <add key="globalPackagesFolder" value="$escapedGlobalPackages" />
  </config>
  <packageSources>
    <clear />
    <add key="local" value="$escapedSource" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
"@

    Set-Content -LiteralPath $Path -Value $content -Encoding utf8NoBOM
}

function Restore-And-BuildPackageFixture {
    param(
        [Parameter(Mandatory = $true)][string]$ProjectRelativePath,
        [Parameter(Mandatory = $true)][string]$CssOutputName,
        [Parameter(Mandatory = $true)][string]$NuGetConfigPath,
        [Parameter(Mandatory = $true)][string]$TempRoot,
        [Parameter(Mandatory = $true)][string]$PackageVersion
    )

    $projectPath = Join-Path $repoRoot $ProjectRelativePath
    $cssOutputDirectory = Join-Path $TempRoot "generated-css"
    $cssOutput = Join-Path $cssOutputDirectory $CssOutputName
    New-Item -ItemType Directory -Force -Path $cssOutputDirectory | Out-Null

    $packageProperties = @(
        "-p:VibeUsePackageReferences=true",
        "-p:VibePackageVersion=$PackageVersion",
        "-p:VibeCssFailOnMissingTool=true"
    )

    Invoke-Checked "dotnet" (@(
        "restore",
        $projectPath,
        "--configfile",
        $NuGetConfigPath
    ) + $packageProperties)

    Invoke-Checked "dotnet" (@(
        "build",
        $projectPath,
        "--configuration",
        "Release",
        "--no-restore",
        "-p:VibeCssOutput=$cssOutput"
    ) + $packageProperties)

    Assert-FileExists $cssOutput
}

function Validate-CliToolPackage {
    param(
        [Parameter(Mandatory = $true)][string]$NuGetConfigPath,
        [Parameter(Mandatory = $true)][string]$LocalSource,
        [Parameter(Mandatory = $true)][string]$TempRoot,
        [Parameter(Mandatory = $true)][string]$PackageVersion
    )

    $toolPath = Join-Path $TempRoot "tools"
    New-Item -ItemType Directory -Force -Path $toolPath | Out-Null

    Invoke-Checked "dotnet" @(
        "tool",
        "install",
        "Vibe.UI.CLI",
        "--version",
        $PackageVersion,
        "--tool-path",
        $toolPath,
        "--configfile",
        $NuGetConfigPath,
        "--add-source",
        $LocalSource
    )

    $runningOnWindows = [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform(
        [System.Runtime.InteropServices.OSPlatform]::Windows)
    $toolExecutable = Join-Path $toolPath $(if ($runningOnWindows) { "vibe.exe" } else { "vibe" })
    Assert-FileExists $toolExecutable

    $cliProject = Join-Path $TempRoot "cli-consumer"
    New-Item -ItemType Directory -Force -Path $cliProject | Out-Null
    Set-Content -LiteralPath (Join-Path $cliProject "CliConsumer.csproj") -Encoding utf8NoBOM -Value @"
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
"@

    Invoke-Checked $toolExecutable @("init", "--yes", "--minimal", "--path", $cliProject)
    Invoke-Checked $toolExecutable @("add", "button", "--yes", "--path", $cliProject)

    Assert-FileExists (Join-Path $cliProject "Vibe/Base/VibeComponent.cs")
    Assert-FileExists (Join-Path $cliProject "Components/vibe/Button.razor")

    $cliCssOutput = Join-Path $TempRoot "cli-generated.css"
    Invoke-Checked $toolExecutable @(
        "css",
        $cliProject,
        "--output",
        $cliCssOutput,
        "--with-base"
    )
    Assert-FileExists $cliCssOutput
}

if ([string]::IsNullOrWhiteSpace($Version)) {
    $Version = Get-ProjectVersion
}

$packagesFullPath = Resolve-PathFromRepo $PackagesPath
if (-not (Test-Path -LiteralPath $packagesFullPath -PathType Container)) {
    throw "Package directory was not found: $packagesFullPath"
}

$packageFiles = @{
    "Vibe.UI.CSS" = Join-Path $packagesFullPath "Vibe.UI.CSS.$Version.nupkg"
    "Vibe.UI" = Join-Path $packagesFullPath "Vibe.UI.$Version.nupkg"
    "Vibe.UI.CLI" = Join-Path $packagesFullPath "Vibe.UI.CLI.$Version.nupkg"
}

foreach ($packageFile in $packageFiles.Values) {
    Assert-FileExists $packageFile
}

$requiredEntries = @{
    "Vibe.UI.CSS" = @(
        "README.css.md",
        "icon.png",
        "buildTransitive/Vibe.UI.CSS.targets",
        "tools/net9.0/Vibe.UI.CSS.dll"
    )
    "Vibe.UI" = @(
        "README.nuget.md",
        "icon.png",
        "lib/net9.0/Vibe.UI.dll",
        "staticwebassets/css/vibe-base.css"
    )
    "Vibe.UI.CLI" = @(
        "README.cli.md",
        "icon.png",
        "tools/net9.0/any/Vibe.UI.CLI.dll",
        "Templates/Infrastructure/ServiceCollectionExtensions.cs"
    )
}

foreach ($packageName in $requiredEntries.Keys) {
    foreach ($entryName in $requiredEntries[$packageName]) {
        Assert-PackageEntry -PackagePath $packageFiles[$packageName] -EntryName $entryName
    }
}

$tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) "vibe-local-package-validation-$([System.Guid]::NewGuid().ToString("N"))"
New-Item -ItemType Directory -Force -Path $tempRoot | Out-Null
$previousNuGetPackages = $env:NUGET_PACKAGES

try {
    $nugetConfig = Join-Path $tempRoot "NuGet.config"
    $globalPackages = Join-Path $tempRoot "nuget-packages"
    Write-NuGetConfig -Path $nugetConfig -LocalSource $packagesFullPath -GlobalPackagesFolder $globalPackages
    $env:NUGET_PACKAGES = $globalPackages

    Restore-And-BuildPackageFixture `
        -ProjectRelativePath "samples/Vibe.UI.Compatibility.StandaloneClient/Vibe.UI.Compatibility.StandaloneClient.csproj" `
        -CssOutputName "standalone.css" `
        -NuGetConfigPath $nugetConfig `
        -TempRoot $tempRoot `
        -PackageVersion $Version

    Restore-And-BuildPackageFixture `
        -ProjectRelativePath "samples/Vibe.UI.Compatibility.WebApp/Vibe.UI.Compatibility.WebApp/Vibe.UI.Compatibility.WebApp.csproj" `
        -CssOutputName "webapp.css" `
        -NuGetConfigPath $nugetConfig `
        -TempRoot $tempRoot `
        -PackageVersion $Version

    Validate-CliToolPackage `
        -NuGetConfigPath $nugetConfig `
        -LocalSource $packagesFullPath `
        -TempRoot $tempRoot `
        -PackageVersion $Version

    Write-Host "Local package validation passed for Vibe.UI $Version."
}
finally {
    if ($null -eq $previousNuGetPackages) {
        Remove-Item Env:NUGET_PACKAGES -ErrorAction SilentlyContinue
    }
    else {
        $env:NUGET_PACKAGES = $previousNuGetPackages
    }

    if ($KeepTemp) {
        Write-Host "Keeping validation temp directory: $tempRoot"
    }
    elseif ((Split-Path -Leaf $tempRoot) -like "vibe-local-package-validation-*") {
        Remove-Item -LiteralPath $tempRoot -Recurse -Force
    }
}
