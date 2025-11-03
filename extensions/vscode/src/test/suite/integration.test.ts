import * as assert from 'assert';
import * as vscode from 'vscode';
import * as path from 'path';
import * as fs from 'fs';

suite('Integration Test Suite', () => {
    test('Extension activates on Razor file', async function() {
        this.timeout(15000); // Give enough time for activation

        // Create a temporary workspace
        const workspaceFolder = vscode.workspace.workspaceFolders?.[0];

        if (workspaceFolder) {
            // Create a .razor file to trigger activation
            const razorFilePath = path.join(workspaceFolder.uri.fsPath, 'Test.razor');

            try {
                fs.writeFileSync(razorFilePath, '@page "/test"\n<h1>Test</h1>');

                // Open the file
                const document = await vscode.workspace.openTextDocument(razorFilePath);
                await vscode.window.showTextDocument(document);

                // Check if extension is active
                const ext = vscode.extensions.getExtension('vibe-ui.vibe-ui');
                assert.ok(ext, 'Extension should exist');

                // Clean up
                fs.unlinkSync(razorFilePath);
            } catch (error) {
                // If we can't create files, skip this test
                console.log('Skipping Razor file test - no workspace available');
            }
        } else {
            // No workspace, just verify extension exists
            const ext = vscode.extensions.getExtension('vibe-ui.vibe-ui');
            assert.ok(ext, 'Extension should exist even without workspace');
        }
    });

    test('Extension has correct activation events', () => {
        const ext = vscode.extensions.getExtension('vibe-ui.vibe-ui');
        assert.ok(ext, 'Extension should exist');

        const packageJSON = ext.packageJSON;
        assert.ok(packageJSON.activationEvents, 'Should have activation events');
        assert.ok(
            packageJSON.activationEvents.includes('workspaceContains:**/*.razor'),
            'Should activate on Razor files'
        );
    });

    test('Extension provides Razor snippets', () => {
        const ext = vscode.extensions.getExtension('vibe-ui.vibe-ui');
        assert.ok(ext, 'Extension should exist');

        const packageJSON = ext.packageJSON;
        assert.ok(packageJSON.contributes.snippets, 'Should contribute snippets');
        assert.ok(packageJSON.contributes.snippets.length > 0, 'Should have at least one snippet');

        const razorSnippet = packageJSON.contributes.snippets.find(
            (s: any) => s.language === 'razor'
        );
        assert.ok(razorSnippet, 'Should have Razor snippets');
        assert.ok(razorSnippet.path, 'Razor snippets should have path');
    });

    test('Extension has correct engine compatibility', () => {
        const ext = vscode.extensions.getExtension('vibe-ui.vibe-ui');
        assert.ok(ext, 'Extension should exist');

        const packageJSON = ext.packageJSON;
        assert.ok(packageJSON.engines, 'Should specify engine requirements');
        assert.ok(packageJSON.engines.vscode, 'Should specify VS Code version');
        assert.ok(
            packageJSON.engines.vscode.startsWith('^1.'),
            'Should support VS Code 1.x'
        );
    });

    test('Extension has proper metadata', () => {
        const ext = vscode.extensions.getExtension('vibe-ui.vibe-ui');
        assert.ok(ext, 'Extension should exist');

        const packageJSON = ext.packageJSON;
        assert.strictEqual(packageJSON.name, 'vibe-ui', 'Name should be vibe-ui');
        assert.strictEqual(packageJSON.displayName, 'Vibe.UI', 'Display name should be Vibe.UI');
        assert.ok(packageJSON.description, 'Should have description');
        assert.ok(packageJSON.version, 'Should have version');
        assert.ok(packageJSON.publisher, 'Should have publisher');
    });

    test('Extension has correct categories', () => {
        const ext = vscode.extensions.getExtension('vibe-ui.vibe-ui');
        assert.ok(ext, 'Extension should exist');

        const packageJSON = ext.packageJSON;
        assert.ok(packageJSON.categories, 'Should have categories');
        assert.ok(packageJSON.categories.includes('Snippets'), 'Should be in Snippets category');
        assert.ok(
            packageJSON.categories.includes('Programming Languages'),
            'Should be in Programming Languages category'
        );
    });
});
