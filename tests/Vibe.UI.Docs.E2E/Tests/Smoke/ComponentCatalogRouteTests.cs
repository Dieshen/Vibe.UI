using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Shouldly;
using Vibe.UI.Docs.E2E.Infrastructure;
using Xunit;

namespace Vibe.UI.Docs.E2E.Tests.Smoke;

[Trait("Category", TestCategories.Smoke)]
public sealed class ComponentCatalogRouteTests : E2ETestBase
{
    [Fact]
    public async Task CatalogListsEveryRoutedComponentPageOnce()
    {
        await NavigateAndWaitForBlazorAsync("/components");

        var links = Page.Locator("a[data-component-route]");
        await links.First.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30000
        });

        var actualRoutes = new List<string>();
        foreach (var link in await links.AllAsync())
        {
            var route = await link.GetAttributeAsync("data-component-route");
            var href = await link.GetAttributeAsync("href");

            route.ShouldNotBeNullOrWhiteSpace("Every component card should identify its route");
            href.ShouldBe(
                route!.TrimStart('/'),
                "Component links must remain relative to the deployed base path");
            actualRoutes.Add(route!);
        }

        actualRoutes.Distinct(StringComparer.OrdinalIgnoreCase).Count().ShouldBe(
            actualRoutes.Count,
            "The component catalog should not contain duplicate route cards");

        var expectedRoutes = ComponentPageManifest.Pages
            .Select(page => page.CanonicalRoute)
            .OrderBy(route => route, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        actualRoutes
            .OrderBy(route => route, StringComparer.OrdinalIgnoreCase)
            .ShouldBe(expectedRoutes);
    }

    [Fact]
    public async Task CatalogLinksResolveWithinANonRootDeploymentBase()
    {
        await NavigateAndWaitForBlazorAsync("/components");

        var links = Page.Locator("a[data-component-route]");
        await links.First.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var resolvedPaths = await links.EvaluateAllAsync<string[]>(
            """
            elements => {
                const deploymentBase = new URL('/Vibe.UI/', window.location.origin);
                return elements.map(element => new URL(element.getAttribute('href'), deploymentBase).pathname);
            }
            """);

        var expectedPaths = ComponentPageManifest.Pages
            .Select(page => $"/Vibe.UI{page.CanonicalRoute}")
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        resolvedPaths
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ShouldBe(expectedPaths);
    }
}

[Trait("Category", TestCategories.Smoke)]
public sealed class ComponentPageManifestTests
{
    [Fact]
    public void RoutedComponentPagesHaveUniqueRoutesAndHeadings()
    {
        ComponentPageManifest.Pages.ShouldNotBeEmpty();

        var routes = ComponentPageManifest.Pages
            .SelectMany(page => page.Routes.Select(route => (route, page.SourceFile)))
            .ToArray();

        var duplicateRoutes = routes
            .GroupBy(item => item.route, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => $"{group.Key}: {string.Join(", ", group.Select(item => item.SourceFile))}")
            .ToArray();

        duplicateRoutes.ShouldBeEmpty(
            $"Component routes must be unique. Found: {string.Join("; ", duplicateRoutes)}");

        ComponentPageManifest.Pages
            .Where(page => string.IsNullOrWhiteSpace(page.Heading))
            .ShouldBeEmpty("Every routed component page must expose a route-specific h1 heading");
    }
}

internal static class ComponentPageManifest
{
    private static readonly Regex RouteDirectivePattern = new(
        "^\\s*@page\\s+\"(?<route>/components/[^\"]+)\"\\s*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);

    private static readonly Regex HeadingPattern = new(
        "<h1\\b[^>]*>\\s*(?<heading>[^<]+?)\\s*</h1>",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);

    internal static IReadOnlyList<ComponentPageDefinition> Pages { get; } = DiscoverPages();

    private static IReadOnlyList<ComponentPageDefinition> DiscoverPages()
    {
        var repositoryRoot = FindRepositoryRoot();
        var componentPagesDirectory = Path.Combine(
            repositoryRoot,
            "samples",
            "Vibe.UI.Docs",
            "Pages",
            "Components");

        return Directory
            .EnumerateFiles(componentPagesDirectory, "*.razor", SearchOption.TopDirectoryOnly)
            .Select(file => CreatePageDefinition(repositoryRoot, file))
            .Where(page => page is not null)
            .Cast<ComponentPageDefinition>()
            .OrderBy(page => page.CanonicalRoute, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static ComponentPageDefinition? CreatePageDefinition(string repositoryRoot, string file)
    {
        var source = File.ReadAllText(file);
        var routes = RouteDirectivePattern
            .Matches(source)
            .Select(match => match.Groups["route"].Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (routes.Length == 0)
        {
            return null;
        }

        var headingMatch = HeadingPattern.Match(source);
        if (!headingMatch.Success)
        {
            throw new InvalidOperationException(
                $"Routed component page '{file}' must contain a plain-text h1 heading for route validation.");
        }

        var pageKey = GetPageKey(Path.GetFileNameWithoutExtension(file));
        var heading = NormalizeWhitespace(headingMatch.Groups["heading"].Value);
        var sourceFile = Path.GetRelativePath(repositoryRoot, file).Replace('\\', '/');

        return new ComponentPageDefinition(
            pageKey,
            heading,
            sourceFile,
            routes,
            SelectCanonicalRoute(routes, pageKey));
    }

    private static string GetPageKey(string fileName)
    {
        if (fileName.EndsWith("View", StringComparison.Ordinal))
        {
            return fileName[..^"View".Length];
        }

        return fileName switch
        {
            "TreeViewer" => "TreeView",
            _ => fileName
        };
    }

    private static string SelectCanonicalRoute(IEnumerable<string> routes, string pageKey)
    {
        var routeList = routes.ToArray();
        var matchingRoute = routeList.FirstOrDefault(route =>
            NormalizeIdentifier(route[(route.LastIndexOf('/') + 1)..]) == NormalizeIdentifier(pageKey));

        return matchingRoute ?? routeList
            .OrderBy(route => route.Length)
            .ThenBy(route => route, StringComparer.OrdinalIgnoreCase)
            .First();
    }

    private static string NormalizeIdentifier(string value) =>
        new(value
            .Where(char.IsLetterOrDigit)
            .Select(char.ToLowerInvariant)
            .ToArray());

    private static string NormalizeWhitespace(string value) =>
        string.Join(' ', value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Vibe.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            $"Could not find Vibe.sln above '{AppContext.BaseDirectory}'.");
    }
}

internal sealed record ComponentPageDefinition(
    string PageKey,
    string Heading,
    string SourceFile,
    IReadOnlyList<string> Routes,
    string CanonicalRoute);
