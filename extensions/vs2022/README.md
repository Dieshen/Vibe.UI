# Vibe.UI for Visual Studio 2022

Official Visual Studio 2022 extension for Vibe.UI - A comprehensive Blazor component library.

## Features

- **Project Templates**: Blazor project templates with Vibe.UI pre-configured
- **Item Templates**: Component templates for common Vibe.UI patterns
- **Code Snippets**: IntelliSense snippets for all components
- **Add Component Command**: Right-click menu to add components
- **Integrated CLI**: Access Vibe.UI CLI commands from within Visual Studio

## Installation

1. Download the VSIX from the Visual Studio Marketplace or GitHub Releases
2. Double-click the VSIX file to install
3. Restart Visual Studio 2022
4. Install the Vibe.UI CLI: `dotnet tool install -g Vibe.UI.CLI`

## Usage

### Creating a New Project

1. File > New > Project
2. Search for "Vibe.UI"
3. Select "Blazor WebAssembly App with Vibe.UI" or "Blazor Server App with Vibe.UI"
4. Configure your project and click Create

### Adding Components

**Method 1: Using the Command**
1. Right-click on your project in Solution Explorer
2. Select "Add Vibe.UI Component"
3. Choose the component from the dialog
4. Click Add

**Method 2: Using Item Templates**
1. Right-click on a folder in Solution Explorer
2. Add > New Item
3. Search for "Vibe"
4. Select a Vibe.UI component template
5. Name your component and click Add

### Using Code Snippets

Type the snippet shortcut (e.g., `vibe-button`) in a Razor file and press `Tab` twice to insert the component.

## Available Templates

### Project Templates
- Blazor WebAssembly App with Vibe.UI
- Blazor Server App with Vibe.UI
- Blazor Hybrid (MAUI) App with Vibe.UI

### Item Templates
- Vibe.UI Component
- Vibe.UI Page
- Vibe.UI Layout

## Requirements

- Visual Studio 2022 (17.0 or later)
- .NET 9.0 SDK
- Vibe.UI CLI

## Support

For issues and feature requests, please visit:
https://github.com/yourusername/Vibe.UI/issues

## License

MIT License - see LICENSE for details
