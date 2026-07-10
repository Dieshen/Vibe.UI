# Blazor Web App Compatibility Fixture

This sample is a Vibe.UI compatibility fixture for a Blazor Web App with server and client projects.

- Interactivity mode: Auto-capable per-page render modes with static SSR, Interactive Server, Interactive WebAssembly, and Interactive Auto routes.
- Register Vibe.UI services in both server and client `Program.cs` files because `.Client` components can prerender on the server before activating in WebAssembly.
- Keep the fixture minimal and focused on hosting compatibility, not full documentation coverage.
