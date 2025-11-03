import * as vscode from 'vscode';
import { exec } from 'child_process';
import { promisify } from 'util';

const execAsync = promisify(exec);

export function activate(context: vscode.ExtensionContext) {
    console.log('Vibe.UI extension is now active');

    // Register Add Component command
    let addComponent = vscode.commands.registerCommand('vibe-ui.addComponent', async () => {
        const components = [
            'button', 'checkbox', 'input', 'select', 'slider', 'switch', 'textarea',
            'card', 'dialog', 'toast', 'avatar', 'badge', 'table', 'chart',
            'accordion', 'tabs', 'carousel', 'tooltip', 'popover', 'dropdown',
            'label', 'togglegroup', 'radiogroup', 'colorpicker'
        ];

        const component = await vscode.window.showQuickPick(components, {
            placeHolder: 'Select a component to add'
        });

        if (component) {
            await addComponentToProject(component);
        }
    });

    // Register Init Project command
    let initProject = vscode.commands.registerCommand('vibe-ui.initProject', async () => {
        await initializeVibeUI();
    });

    // Register Open Documentation command
    let openDocs = vscode.commands.registerCommand('vibe-ui.openDocs', () => {
        vscode.env.openExternal(vscode.Uri.parse('https://github.com/yourusername/Vibe.UI#readme'));
    });

    context.subscriptions.push(addComponent, initProject, openDocs);
}

async function addComponentToProject(componentName: string) {
    const workspaceFolder = vscode.workspace.workspaceFolders?.[0];

    if (!workspaceFolder) {
        vscode.window.showErrorMessage('No workspace folder open');
        return;
    }

    try {
        await vscode.window.withProgress({
            location: vscode.ProgressLocation.Notification,
            title: `Adding ${componentName} component...`,
            cancellable: false
        }, async (progress) => {
            const { stdout, stderr } = await execAsync(
                `vibe add ${componentName} -y`,
                { cwd: workspaceFolder.uri.fsPath }
            );

            if (stderr) {
                throw new Error(stderr);
            }

            vscode.window.showInformationMessage(
                `${componentName} component added successfully!`
            );
        });
    } catch (error: any) {
        vscode.window.showErrorMessage(
            `Failed to add component: ${error.message}`
        );
    }
}

async function initializeVibeUI() {
    const workspaceFolder = vscode.workspace.workspaceFolders?.[0];

    if (!workspaceFolder) {
        vscode.window.showErrorMessage('No workspace folder open');
        return;
    }

    const theme = await vscode.window.showQuickPick(['light', 'dark', 'both'], {
        placeHolder: 'Select a theme'
    });

    if (!theme) {
        return;
    }

    try {
        await vscode.window.withProgress({
            location: vscode.ProgressLocation.Notification,
            title: 'Initializing Vibe.UI...',
            cancellable: false
        }, async (progress) => {
            const { stdout, stderr } = await execAsync(
                `vibe init -y`,
                { cwd: workspaceFolder.uri.fsPath }
            );

            if (stderr) {
                throw new Error(stderr);
            }

            vscode.window.showInformationMessage(
                'Vibe.UI initialized successfully!'
            );
        });
    } catch (error: any) {
        vscode.window.showErrorMessage(
            `Failed to initialize Vibe.UI: ${error.message}`
        );
    }
}

export function deactivate() {}
