import * as assert from 'assert';
import * as vscode from 'vscode';
import * as sinon from 'sinon';

suite('Extension Test Suite', () => {
    vscode.window.showInformationMessage('Start all tests.');

    test('Extension should be present', () => {
        assert.ok(vscode.extensions.getExtension('vibe-ui.vibe-ui'));
    });

    test('Extension should activate', async function() {
        this.timeout(10000); // Activation may take time

        const ext = vscode.extensions.getExtension('vibe-ui.vibe-ui');
        assert.ok(ext, 'Extension should be found');

        await ext.activate();
        assert.strictEqual(ext.isActive, true, 'Extension should be active');
    });

    test('All commands should be registered', async () => {
        const commands = await vscode.commands.getCommands(true);

        assert.ok(
            commands.includes('vibe-ui.addComponent'),
            'vibe-ui.addComponent command should be registered'
        );

        assert.ok(
            commands.includes('vibe-ui.initProject'),
            'vibe-ui.initProject command should be registered'
        );

        assert.ok(
            commands.includes('vibe-ui.openDocs'),
            'vibe-ui.openDocs command should be registered'
        );
    });

    test('Configuration should have expected properties', () => {
        const config = vscode.workspace.getConfiguration('vibeUI');

        // Check that our configuration properties exist
        const componentsDir = config.get('componentsDirectory');
        const theme = config.get('theme');

        assert.ok(componentsDir !== undefined, 'componentsDirectory config should exist');
        assert.ok(theme !== undefined, 'theme config should exist');
    });

    test('Configuration should have correct defaults', () => {
        const config = vscode.workspace.getConfiguration('vibeUI');

        const componentsDir = config.get('componentsDirectory');
        const theme = config.get('theme');

        assert.strictEqual(componentsDir, 'Components', 'Default componentsDirectory should be "Components"');
        assert.strictEqual(theme, 'light', 'Default theme should be "light"');
    });
});
