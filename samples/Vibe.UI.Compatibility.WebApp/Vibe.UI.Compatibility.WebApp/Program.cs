using Vibe.UI.Compatibility.WebApp.Components;
using Microsoft.AspNetCore.DataProtection;
using Vibe.UI;

var builder = WebApplication.CreateBuilder(args);

var dataProtectionPath = Environment.GetEnvironmentVariable("VIBE_COMPATIBILITY_DATA_PROTECTION_PATH");
if (!string.IsNullOrWhiteSpace(dataProtectionPath))
{
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath));
}

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddVibeUI();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Vibe.UI.Compatibility.WebApp.Client._Imports).Assembly);

app.Run();
