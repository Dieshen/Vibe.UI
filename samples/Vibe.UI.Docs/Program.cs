using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Vibe.UI;
using Vibe.UI.Docs;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add Vibe.UI services (Toast and Dialog only - theming is pure CSS now)
builder.Services.AddVibeUI(options =>
{
    options.BaseColor = "Slate";
});

// Add LocalStorage for general app use
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
