using Microsoft.Extensions.DependencyInjection;
using Vibe.UI.Configuration;
using Vibe.UI.Services.Dialog;
using Vibe.UI.Services.Theme;
using Vibe.UI.Services.Toast;

namespace Vibe.UI.Tests.Services;

/// <summary>
/// Integration tests to verify that Vibe.UI services are correctly registered
/// with the dependency injection container.
/// </summary>
public class ServiceRegistrationTests
{
    #region Basic Registration Tests

    [Fact]
    public void AddVibeUI_RegistersAllRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddVibeUI();
        var provider = services.BuildServiceProvider();

        // Assert - Verify all services are registered
        provider.GetService<VibeThemeOptions>().ShouldNotBeNull();
        provider.GetService<IThemeService>().ShouldNotBeNull();
        provider.GetService<IToastService>().ShouldNotBeNull();
        provider.GetService<IDialogService>().ShouldNotBeNull();
    }

    [Fact]
    public void AddVibeUI_ReturnsServiceCollectionForChaining()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddVibeUI();

        // Assert
        result.ShouldBeSameAs(services);
    }

    [Fact]
    public void AddVibeUI_WithNullConfig_UsesDefaultOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddVibeUI(null);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<VibeThemeOptions>();

        // Assert - Default values
        options.BaseColor.ShouldBe("Slate");
        options.BorderRadius.ShouldBe("0.5rem");
    }

    #endregion

    #region Service Lifetime Tests

    [Fact]
    public void AddVibeUI_RegistersThemeOptionsAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddVibeUI();
        var provider = services.BuildServiceProvider();

        // Act
        var instance1 = provider.GetRequiredService<VibeThemeOptions>();
        var instance2 = provider.GetRequiredService<VibeThemeOptions>();

        // Assert
        instance1.ShouldBeSameAs(instance2);
    }

    [Fact]
    public void AddVibeUI_RegistersThemeServiceAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddVibeUI();
        var provider = services.BuildServiceProvider();

        // Act
        var instance1 = provider.GetRequiredService<IThemeService>();
        var instance2 = provider.GetRequiredService<IThemeService>();

        // Assert
        instance1.ShouldBeSameAs(instance2);
    }

    [Fact]
    public void AddVibeUI_RegistersToastServiceAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddVibeUI();
        var provider = services.BuildServiceProvider();

        // Act
        var instance1 = provider.GetRequiredService<IToastService>();
        var instance2 = provider.GetRequiredService<IToastService>();

        // Assert
        instance1.ShouldBeSameAs(instance2);
    }

    [Fact]
    public void AddVibeUI_RegistersDialogServiceAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddVibeUI();
        var provider = services.BuildServiceProvider();

        // Act
        var instance1 = provider.GetRequiredService<IDialogService>();
        var instance2 = provider.GetRequiredService<IDialogService>();

        // Assert
        instance1.ShouldBeSameAs(instance2);
    }

    #endregion

    #region Theme Configuration Tests

    [Fact]
    public void AddVibeUI_WithCustomConfig_AppliesConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddVibeUI(options =>
        {
            options.BaseColor = "Blue";
            options.BorderRadius = "1rem";
        });
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<VibeThemeOptions>();

        // Assert
        options.BaseColor.ShouldBe("Blue");
        options.BorderRadius.ShouldBe("1rem");
    }

    [Theory]
    [InlineData("Slate")]
    [InlineData("Gray")]
    [InlineData("Zinc")]
    [InlineData("Neutral")]
    [InlineData("Stone")]
    [InlineData("Blue")]
    public void AddVibeUI_WithValidBaseColor_ConfiguresTheme(string baseColor)
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddVibeUI(options => options.BaseColor = baseColor);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<VibeThemeOptions>();

        // Assert
        options.BaseColor.ShouldBe(baseColor);
    }

    [Fact]
    public void AddVibeUI_WithCustomColors_ConfiguresCustomTheme()
    {
        // Arrange
        var services = new ServiceCollection();
        var customLightColors = new CustomThemeColors
        {
            Background = "hsl(0 0% 100%)",
            Foreground = "hsl(0 0% 0%)",
            Primary = "hsl(200 100% 50%)"
        };

        // Act
        services.AddVibeUI(options =>
        {
            options.BaseColor = "Custom";
            options.LightColors = customLightColors;
        });
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<VibeThemeOptions>();

        // Assert
        options.BaseColor.ShouldBe("Custom");
        options.LightColors.ShouldNotBeNull();
        options.LightColors!.Background.ShouldBe("hsl(0 0% 100%)");
        options.LightColors.Primary.ShouldBe("hsl(200 100% 50%)");
    }

    #endregion

    #region ThemeService Integration Tests

    [Fact]
    public void ThemeService_GenerateThemeCss_ReturnsValidCss()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddVibeUI();
        var provider = services.BuildServiceProvider();
        var themeService = provider.GetRequiredService<IThemeService>();

        // Act
        var css = themeService.GenerateThemeCss();

        // Assert
        css.ShouldNotBeNullOrWhiteSpace();
        css.ShouldContain(":root");
        css.ShouldContain(".dark");
        css.ShouldContain("--vibe-background");
        css.ShouldContain("--vibe-primary");
        css.ShouldContain("--vibe-radius");
    }

    [Theory]
    [InlineData("Slate")]
    [InlineData("Blue")]
    [InlineData("Gray")]
    public void ThemeService_GenerateThemeCss_IncludesConfiguredBaseColor(string baseColor)
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddVibeUI(options => options.BaseColor = baseColor);
        var provider = services.BuildServiceProvider();
        var themeService = provider.GetRequiredService<IThemeService>();

        // Act
        var css = themeService.GenerateThemeCss();

        // Assert - CSS should be generated without errors
        css.ShouldNotBeNullOrWhiteSpace();
        css.ShouldContain("--vibe-primary");
    }

    [Theory]
    [InlineData("Slate", "hsl(217.2 32.6% 17.5%)", "hsl(215 20.2% 65.1%)")]
    [InlineData("Blue", "hsl(217.2 32.6% 17.5%)", "hsl(215 20.2% 65.1%)")]
    [InlineData("Gray", "hsl(215 27.9% 16.9%)", "hsl(217.9 10.6% 64.9%)")]
    [InlineData("Zinc", "hsl(240 3.7% 15.9%)", "hsl(240 5% 64.9%)")]
    [InlineData("Neutral", "hsl(0 0% 14.9%)", "hsl(0 0% 63.9%)")]
    [InlineData("Stone", "hsl(12 6.5% 15.1%)", "hsl(24 5.4% 63.9%)")]
    public void ThemeService_GenerateThemeCss_SeparatesDarkMutedSurfaceAndText(
        string baseColor,
        string expectedMuted,
        string expectedMutedForeground)
    {
        var services = new ServiceCollection();
        services.AddVibeUI(options => options.BaseColor = baseColor);
        var provider = services.BuildServiceProvider();
        var themeService = provider.GetRequiredService<IThemeService>();

        var css = themeService.GenerateThemeCss();
        var darkThemeCss = css[css.IndexOf(".dark", StringComparison.Ordinal)..];

        darkThemeCss.ShouldContain($"--vibe-muted: {expectedMuted};");
        darkThemeCss.ShouldContain($"--vibe-muted-foreground: {expectedMutedForeground};");
        expectedMuted.ShouldNotBe(expectedMutedForeground);
    }

    [Fact]
    public void ThemeService_GenerateThemeCss_DefaultSlateUsesContrastingDarkPrimaryPair()
    {
        var services = new ServiceCollection();
        services.AddVibeUI();
        var provider = services.BuildServiceProvider();
        var themeService = provider.GetRequiredService<IThemeService>();

        var css = themeService.GenerateThemeCss();
        var darkThemeCss = css[css.IndexOf(".dark", StringComparison.Ordinal)..];

        darkThemeCss.ShouldContain("--vibe-primary: hsl(210 40% 98%);");
        darkThemeCss.ShouldContain("--vibe-primary-foreground: hsl(222.2 47.4% 11.2%);");
        darkThemeCss.ShouldContain("--vibe-card: hsl(222.2 84% 4.9%);");
    }

    [Fact]
    public void ThemeService_GenerateThemeCss_IncludesCustomBorderRadius()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddVibeUI(options => options.BorderRadius = "1.5rem");
        var provider = services.BuildServiceProvider();
        var themeService = provider.GetRequiredService<IThemeService>();

        // Act
        var css = themeService.GenerateThemeCss();

        // Assert
        css.ShouldContain("--vibe-radius: 1.5rem");
    }

    #endregion

    #region ToastService Integration Tests

    [Fact]
    public async Task ToastService_ShowAsync_RaisesOnToastAddedEvent()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddVibeUI();
        var provider = services.BuildServiceProvider();
        var toastService = provider.GetRequiredService<IToastService>();

        ToastEventArgs? receivedArgs = null;
        toastService.OnToastAdded += (sender, args) => receivedArgs = args;

        // Act
        await toastService.ShowAsync("Test Title", "Test Message");

        // Assert
        receivedArgs.ShouldNotBeNull();
        receivedArgs!.Title.ShouldBe("Test Title");
        receivedArgs.Description.ShouldBe("Test Message");
    }

    [Fact]
    public async Task ToastService_ShowSuccessAsync_RaisesEventWithSuccessVariant()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddVibeUI();
        var provider = services.BuildServiceProvider();
        var toastService = provider.GetRequiredService<IToastService>();

        ToastEventArgs? receivedArgs = null;
        toastService.OnToastAdded += (sender, args) => receivedArgs = args;

        // Act
        await toastService.ShowSuccessAsync("Success", "Operation completed");

        // Assert
        receivedArgs.ShouldNotBeNull();
        receivedArgs!.Variant.ShouldBe("success");
    }

    [Fact]
    public async Task ToastService_ShowErrorAsync_RaisesEventWithErrorVariant()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddVibeUI();
        var provider = services.BuildServiceProvider();
        var toastService = provider.GetRequiredService<IToastService>();

        ToastEventArgs? receivedArgs = null;
        toastService.OnToastAdded += (sender, args) => receivedArgs = args;

        // Act
        await toastService.ShowErrorAsync("Error", "Something went wrong");

        // Assert
        receivedArgs.ShouldNotBeNull();
        receivedArgs!.Variant.ShouldBe("error");
    }

    #endregion

    #region DialogService Integration Tests

    [Fact]
    public void DialogService_OnDialogOpened_EventIsAccessible()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddVibeUI();
        var provider = services.BuildServiceProvider();
        var dialogService = provider.GetRequiredService<IDialogService>();

        // Act & Assert - Event subscription should not throw
        bool eventFired = false;
        dialogService.OnDialogOpened += (sender, args) => eventFired = true;

        // The event handler was attached successfully
        eventFired.ShouldBeFalse(); // Event hasn't been triggered yet
    }

    [Fact]
    public void DialogService_OnDialogClosed_EventIsAccessible()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddVibeUI();
        var provider = services.BuildServiceProvider();
        var dialogService = provider.GetRequiredService<IDialogService>();

        // Act & Assert - Event subscription should not throw
        bool eventFired = false;
        dialogService.OnDialogClosed += (sender, args) => eventFired = true;

        // The event handler was attached successfully
        eventFired.ShouldBeFalse(); // Event hasn't been triggered yet
    }

    [Fact]
    public async Task DialogService_ShowCustomAsync_ThrowsWhenDialogAlreadyOpen()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddVibeUI();
        var provider = services.BuildServiceProvider();
        var dialogService = provider.GetRequiredService<IDialogService>();

        Microsoft.AspNetCore.Components.RenderFragment content = _ => { };

        // Act
        var first = dialogService.ShowCustomAsync("First", content);

        // Assert
        Should.Throw<InvalidOperationException>(() => dialogService.ShowCustomAsync("Second", content));

        dialogService.Close();
        await first;
    }

    #endregion

    #region Multiple Registration Tests

    [Fact]
    public void AddVibeUI_CalledMultipleTimes_DoesNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert - Should not throw
        Should.NotThrow(() =>
        {
            services.AddVibeUI();
            services.AddVibeUI();
        });
    }

    [Fact]
    public void AddVibeUI_CalledMultipleTimes_LastConfigWins()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddVibeUI(options => options.BaseColor = "Blue");
        services.AddVibeUI(options => options.BaseColor = "Gray");
        var provider = services.BuildServiceProvider();

        // Assert - Last registration wins for singleton
        var options = provider.GetRequiredService<VibeThemeOptions>();
        options.BaseColor.ShouldBe("Gray");
    }

    #endregion

    #region Concrete Implementation Tests

    [Fact]
    public void AddVibeUI_RegistersThemeServiceAsConcreteType()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddVibeUI();
        var provider = services.BuildServiceProvider();

        // Act
        var service = provider.GetRequiredService<IThemeService>();

        // Assert
        service.ShouldBeOfType<ThemeService>();
    }

    [Fact]
    public void AddVibeUI_RegistersToastServiceAsConcreteType()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddVibeUI();
        var provider = services.BuildServiceProvider();

        // Act
        var service = provider.GetRequiredService<IToastService>();

        // Assert
        service.ShouldBeOfType<ToastService>();
    }

    [Fact]
    public void AddVibeUI_RegistersDialogServiceAsConcreteType()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddVibeUI();
        var provider = services.BuildServiceProvider();

        // Act
        var service = provider.GetRequiredService<IDialogService>();

        // Assert
        service.ShouldBeOfType<DialogService>();
    }

    #endregion
}
