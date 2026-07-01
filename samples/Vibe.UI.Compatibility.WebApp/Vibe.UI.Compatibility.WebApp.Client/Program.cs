using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Vibe.UI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddVibeUI();

await builder.Build().RunAsync();
