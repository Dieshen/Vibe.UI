# Vibe.UI for Visual Studio Code

Official Visual Studio Code extension for Vibe.UI - A comprehensive Blazor component library.

## Features

- **Quick Component Addition**: Add Vibe.UI components to your project with a simple command
- **IntelliSense Support**: Code snippets for all Vibe.UI components
- **Project Initialization**: Initialize Vibe.UI in your Blazor project
- **Documentation Access**: Quick access to component documentation

## Commands

- `Vibe.UI: Add Component` - Add a component to your project
- `Vibe.UI: Initialize Project` - Initialize Vibe.UI in your project
- `Vibe.UI: Open Documentation` - Open Vibe.UI documentation

## Snippets

This extension includes code snippets for all Vibe.UI components:

- `vibe-button` - Button component
- `vibe-input` - Input component
- `vibe-card` - Card component
- `vibe-dialog` - Dialog component
- `vibe-badge` - Badge component
- `vibe-label` - Label component
- `vibe-togglegroup` - Toggle Group component
- `vibe-radiogroup` - Radio Group component
- `vibe-tabs` - Tabs component
- `vibe-accordion` - Accordion component

And many more!

## Requirements

- Visual Studio Code 1.85.0 or higher
- .NET 9.0 SDK
- Vibe.UI CLI (`dotnet tool install -g Vibe.UI.CLI`)

## Installation

1. Install the extension from the VS Code marketplace
2. Install the Vibe.UI CLI: `dotnet tool install -g Vibe.UI.CLI`
3. Open a Blazor project
4. Run `Vibe.UI: Initialize Project` command

## Usage

### Adding a Component

1. Open the command palette (`Ctrl+Shift+P` or `Cmd+Shift+P`)
2. Type `Vibe.UI: Add Component`
3. Select the component you want to add
4. The component will be added to your project

### Using Snippets

Type the snippet prefix (e.g., `vibe-button`) in a Razor file and press `Tab` to insert the component template.

## License

MIT
