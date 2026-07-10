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

function Copy-DirectoryTree {
    param(
        [Parameter(Mandatory = $true)][string]$SourcePath,
        [Parameter(Mandatory = $true)][string]$DestinationPath
    )

    New-Item -ItemType Directory -Force -Path $DestinationPath | Out-Null
    Copy-Item -Path (Join-Path $SourcePath '*') -Destination $DestinationPath -Recurse -Force
}

function Restore-And-BuildPackageFixture {
    param(
        [Parameter(Mandatory = $true)][string]$FixtureRelativePath,
        [Parameter(Mandatory = $true)][string]$ProjectRelativePath,
        [Parameter(Mandatory = $true)][string]$NuGetConfigPath,
        [Parameter(Mandatory = $true)][string]$TempRoot,
        [Parameter(Mandatory = $true)][string]$PackageVersion,
        [Parameter(Mandatory = $true)][string[]]$ExpectedCssOutputs,
        [string[]]$ExpectedMissingCssOutputs = @(),
        [int]$BuildCount = 1
    )

    $fixtureSourcePath = Join-Path $repoRoot $FixtureRelativePath
    $fixtureName = [System.IO.Path]::GetFileName($FixtureRelativePath.TrimEnd('\', '/'))
    $fixtureRoot = Join-Path $TempRoot $fixtureName
    Copy-DirectoryTree -SourcePath $fixtureSourcePath -DestinationPath $fixtureRoot

    $projectPath = Join-Path $fixtureRoot $ProjectRelativePath

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
        "--no-restore"
    ) + $packageProperties)

    foreach ($relativePath in $ExpectedCssOutputs) {
        Assert-FileExists (Join-Path $fixtureRoot $relativePath)
    }

    foreach ($relativePath in $ExpectedMissingCssOutputs) {
        $fullPath = Join-Path $fixtureRoot $relativePath
        if (Test-Path -LiteralPath $fullPath -PathType Leaf) {
            throw "Expected CSS output to be absent: $fullPath"
        }
    }

    for ($buildIndex = 2; $buildIndex -le $BuildCount; $buildIndex++) {
        Invoke-Checked "dotnet" (@(
            "build",
            $projectPath,
            "--configuration",
            "Release",
            "--no-restore"
        ) + $packageProperties)
    }

    foreach ($relativePath in $ExpectedMissingCssOutputs) {
        $fullPath = Join-Path $fixtureRoot $relativePath
        if (Test-Path -LiteralPath $fullPath -PathType Leaf) {
            throw "Expected CSS output to remain absent after rebuild: $fullPath"
        }
    }
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
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>
</Project>
"@

    Invoke-Checked $toolExecutable @("init", "--yes", "--minimal", "--path", $cliProject)

    $rootImportsPath = Join-Path $cliProject "_Imports.razor"
    if (Test-Path -LiteralPath $rootImportsPath -PathType Leaf) {
        Remove-Item -LiteralPath $rootImportsPath -Force
    }

    $componentImportsPath = Join-Path $cliProject "Components/_Imports.razor"
    Set-Content -LiteralPath $componentImportsPath -Encoding utf8NoBOM -Value @"
@using global::Vibe.UI.Base
@using global::Vibe.UI.Enums
@using global::Microsoft.AspNetCore.Components.Web
@using global::Microsoft.JSInterop
"@

    Invoke-Checked $toolExecutable @("add", "button", "--yes", "--path", $cliProject)

    Assert-FileExists (Join-Path $cliProject "Vibe/Base/VibeComponent.cs")
    Assert-FileExists (Join-Path $cliProject "Components/vibe/Button.razor")
    Assert-FileExists (Join-Path $cliProject "wwwroot/js/vibe-dialog.js")
    Assert-FileExists (Join-Path $cliProject "wwwroot/js/vibe-richtext.js")

    Invoke-Checked $toolExecutable @("add", "dialog", "--yes", "--path", $cliProject)

    $dialogComponentPath = Join-Path $cliProject "Components/vibe/Dialog.razor"
    Assert-FileExists $dialogComponentPath

    $dialogComponentContent = Get-Content -LiteralPath $dialogComponentPath -Raw
    if ($dialogComponentContent -notmatch '\./js/vibe-dialog\.js') {
        throw "Expected generated dialog component to reference ./js/vibe-dialog.js."
    }

    if ($dialogComponentContent -match '_content/Vibe\.UI/js') {
        throw "Generated dialog component still references _content/Vibe.UI/js."
    }

    $cliCssOutput = Join-Path $TempRoot "cli-generated.css"
    Invoke-Checked $toolExecutable @(
        "css",
        $cliProject,
        "--output",
        $cliCssOutput,
        "--with-base"
    )
    Assert-FileExists $cliCssOutput

    Invoke-Checked "dotnet" @(
        "restore",
        (Join-Path $cliProject "CliConsumer.csproj"),
        "--configfile",
        $NuGetConfigPath
    )

    Invoke-Checked "dotnet" @(
        "build",
        (Join-Path $cliProject "CliConsumer.csproj"),
        "--configuration",
        "Release",
        "--no-restore",
        "-p:TreatWarningsAsErrors=true"
    )

    $hostedRoot = Join-Path $TempRoot "cli-hosted"
    Invoke-Checked "dotnet" @(
        "new",
        "blazor",
        "--name",
        "CliHosted",
        "--output",
        $hostedRoot,
        "--framework",
        "net10.0",
        "--interactivity",
        "WebAssembly",
        "--all-interactive",
        "--no-restore",
        "--no-update-check"
    )

    Invoke-Checked $toolExecutable @(
        "init",
        "--yes",
        "--minimal",
        "--with-css",
        "--path",
        $hostedRoot
    )

    $hostedServerProject = Join-Path $hostedRoot "CliHosted/CliHosted.csproj"
    $hostedClientProject = Join-Path $hostedRoot "CliHosted.Client/CliHosted.Client.csproj"
    $hostedServerContent = Get-Content -LiteralPath $hostedServerProject -Raw
    $hostedClientContent = Get-Content -LiteralPath $hostedClientProject -Raw

    if ($hostedServerContent -notmatch '<PackageReference Include="Vibe\.UI\.CSS"') {
        throw "Expected hosted CLI initialization to add Vibe.UI.CSS to the server project."
    }

    if (-not $hostedServerContent.Contains('<VibeCssScanRoot>$(MSBuildProjectDirectory)/..</VibeCssScanRoot>')) {
        throw "Expected hosted CLI initialization to configure the server shared CSS scan root."
    }

    if ($hostedClientContent -match '<PackageReference Include="Vibe\.UI\.CSS"') {
        throw "Hosted CLI initialization must not add Vibe.UI.CSS to the client project."
    }

    Assert-FileExists (Join-Path $hostedRoot "CliHosted.Client/vibe.json")

    Invoke-Checked "dotnet" @(
        "restore",
        $hostedServerProject,
        "--configfile",
        $NuGetConfigPath
    )

    for ($buildIndex = 1; $buildIndex -le 2; $buildIndex++) {
        Invoke-Checked "dotnet" @(
            "build",
            $hostedServerProject,
            "--configuration",
            "Release",
            "--no-restore",
            "-p:TreatWarningsAsErrors=true"
        )
    }

    Assert-FileExists (Join-Path $hostedRoot "CliHosted/wwwroot/css/Vibe.UI.CSS")

    $hostedClientCss = Join-Path $hostedRoot "CliHosted.Client/wwwroot/css/Vibe.UI.CSS"
    if (Test-Path -LiteralPath $hostedClientCss -PathType Leaf) {
        throw "Hosted CLI initialization generated a duplicate client Vibe.UI.CSS asset."
    }
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
        "tools/net10.0/Vibe.UI.CSS.dll"
    )
    "Vibe.UI" = @(
        "README.nuget.md",
        "icon.png",
        "lib/net10.0/Vibe.UI.dll",
        "staticwebassets/css/vibe-base.css"
    )
    "Vibe.UI.CLI" = @(
        "README.cli.md",
        "icon.png",
        "tools/net10.0/any/Vibe.UI.CLI.dll",
        "Templates/Infrastructure/ServiceCollectionExtensions.cs",
        "Templates/wwwroot/js/vibe-dialog.js",
        "Templates/wwwroot/js/vibe-richtext.js"
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
        -FixtureRelativePath "samples/Vibe.UI.Compatibility.StandaloneClient" `
        -ProjectRelativePath "Vibe.UI.Compatibility.StandaloneClient.csproj" `
        -NuGetConfigPath $nugetConfig `
        -TempRoot $tempRoot `
        -PackageVersion $Version `
        -ExpectedCssOutputs @("wwwroot/css/Vibe.UI.CSS")

    Restore-And-BuildPackageFixture `
        -FixtureRelativePath "samples/Vibe.UI.Compatibility.WebApp" `
        -ProjectRelativePath "Vibe.UI.Compatibility.WebApp/Vibe.UI.Compatibility.WebApp.csproj" `
        -NuGetConfigPath $nugetConfig `
        -TempRoot $tempRoot `
        -PackageVersion $Version `
        -ExpectedCssOutputs @("Vibe.UI.Compatibility.WebApp/wwwroot/css/Vibe.UI.CSS") `
        -ExpectedMissingCssOutputs @("Vibe.UI.Compatibility.WebApp.Client/wwwroot/css/Vibe.UI.CSS") `
        -BuildCount 2

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
