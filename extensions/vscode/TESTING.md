# VS Code Extension Testing

## Overview

The VS Code extension uses TypeScript and requires the VS Code Extension Test Runner for comprehensive testing.

## Test Requirements

### Prerequisites
- Node.js 18+
- VS Code Extension Test Runner
- Mocha test framework

## Setting Up Tests

### Install Dependencies

```bash
cd extensions/vscode
npm install --save-dev @vscode/test-electron mocha @types/mocha chai @types/chai
```

### Test Structure

Create `src/test/` directory:
```
src/test/
├── suite/
│   ├── extension.test.ts
│   ├── commands.test.ts
│   └── integration.test.ts
├── runTest.ts
└── index.ts
```

## Test Examples

### Extension Activation Test

```typescript
import * as assert from 'assert';
import * as vscode from 'vscode';

suite('Extension Test Suite', () => {
    test('Extension should be present', () => {
        assert.ok(vscode.extensions.getExtension('vibe-ui.vscode-extension'));
    });

    test('Extension should activate', async () => {
        const ext = vscode.extensions.getExtension('vibe-ui.vscode-extension');
        await ext?.activate();
        assert.ok(ext?.isActive);
    });
});
```

### Command Tests

```typescript
suite('Command Tests', () => {
    test('vibe-ui.addComponent command is registered', async () => {
        const commands = await vscode.commands.getCommands(true);
        assert.ok(commands.includes('vibe-ui.addComponent'));
    });

    test('vibe-ui.initProject command is registered', async () => {
        const commands = await vscode.commands.getCommands(true);
        assert.ok(commands.includes('vibe-ui.initProject'));
    });

    test('vibe-ui.openDocs command is registered', async () => {
        const commands = await vscode.commands.getCommands(true);
        assert.ok(commands.includes('vibe-ui.openDocs'));
    });
});
```

### Integration Tests

```typescript
suite('Integration Tests', () => {
    test('addComponent executes vibe CLI', async () => {
        // Mock workspace
        // Execute command
        // Verify CLI was called
    });

    test('initProject creates configuration', async () => {
        // Mock workspace
        // Execute init command
        // Verify vibe.json was created
    });
});
```

## Test Coverage Areas

### Unit Tests

1. **Extension Activation**
   - Extension loads correctly
   - All commands are registered
   - Activation events work properly

2. **Command Registration**
   - `vibe-ui.addComponent` exists
   - `vibe-ui.initProject` exists
   - `vibe-ui.openDocs` exists

3. **Command Execution**
   - Components can be selected from quick pick
   - CLI is invoked with correct arguments
   - Error handling for failed CLI commands
   - Progress notifications display correctly

4. **Helper Functions**
   - `addComponentToProject()` executes correctly
   - `initializeVibeUI()` creates configuration
   - Error messages display properly

### Integration Tests

1. **CLI Integration**
   - Vibe CLI is executed successfully
   - Working directory is set correctly
   - Output is captured and displayed

2. **Workspace Integration**
   - Workspace folder detection
   - File system operations
   - Configuration file creation

## Running Tests

### Command Line

```bash
npm test
```

### VS Code

1. Open the extension in VS Code
2. Press F5 to launch Extension Development Host
3. Open Command Palette (Ctrl+Shift+P)
4. Run: "Developer: Run Extension Tests"

## Manual Testing Checklist

- [ ] Extension activates on Blazor project open
- [ ] Add Component command appears in Command Palette
- [ ] Component picker shows all available components
- [ ] Selected component is added via CLI
- [ ] Progress notification displays during installation
- [ ] Success notification displays on completion
- [ ] Error notification displays on failure
- [ ] Init Project command works correctly
- [ ] Theme selection works
- [ ] Open Docs command opens browser
- [ ] Extension works in Windows, macOS, and Linux

## Test Configuration

### package.json Scripts

```json
{
  "scripts": {
    "test": "node ./out/test/runTest.js",
    "pretest": "npm run compile",
    "compile-tests": "tsc -p ./"
  }
}
```

### .vscode/launch.json

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Extension Tests",
      "type": "extensionHost",
      "request": "launch",
      "args": [
        "--extensionDevelopmentPath=${workspaceFolder}",
        "--extensionTestsPath=${workspaceFolder}/out/test/suite/index"
      ],
      "outFiles": [
        "${workspaceFolder}/out/test/**/*.js"
      ]
    }
  ]
}
```

## Coverage Goals

Target coverage: 85%+

### Current Status
- Extension activation: 0% (needs tests)
- Command registration: 0% (needs tests)
- addComponentToProject: 0% (needs tests)
- initializeVibeUI: 0% (needs tests)

## Mocking Strategy

### Mock exec for CLI commands

```typescript
import * as sinon from 'sinon';
import { exec } from 'child_process';

suite('Mocked CLI Tests', () => {
    let execStub: sinon.SinonStub;

    setup(() => {
        execStub = sinon.stub(require('child_process'), 'exec');
    });

    teardown(() => {
        execStub.restore();
    });

    test('CLI success path', async () => {
        execStub.yields(null, 'success output', '');
        // Test command execution
    });
});
```

## CI/CD Integration

### GitHub Actions Workflow

```yaml
name: Test VS Code Extension

on: [push, pull_request]

jobs:
  test:
    runs-on: ${{ matrix.os }}
    strategy:
      matrix:
        os: [ubuntu-latest, windows-latest, macos-latest]
        node-version: [18.x]
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-node@v3
        with:
          node-version: ${{ matrix.node-version }}
      - run: npm ci
      - run: npm test
        env:
          DISPLAY: ':99.0'
```

## Future Improvements

1. Add comprehensive unit tests for all commands
2. Add integration tests with mock workspace
3. Add E2E tests for full user workflows
4. Set up code coverage reporting
5. Add performance tests
6. Implement visual regression testing

## Resources

- [VS Code Extension Testing Guide](https://code.visualstudio.com/api/working-with-extensions/testing-extension)
- [Extension Test Runner](https://github.com/microsoft/vscode-test)
- [Mocha Testing Framework](https://mochajs.org/)
