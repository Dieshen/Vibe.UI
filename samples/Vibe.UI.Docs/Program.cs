using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Vibe.UI.Docs;
using Vibe.UI;
using Blazored.LocalStorage;
using Vibe.UI.Themes.Models;
using Vibe.UI.Themes.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add Vibe.UI services with theme configuration
builder.Services.AddVibeUI(options =>
{
    options.PersistTheme = true;
    options.DefaultThemeId = "light";
    options.IncludeBuiltInThemes = new List<string> { "light", "dark" };
});

// Add LocalStorage for theme persistence
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
