using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace Vibe.UI.VS2022.Commands
{
    /// <summary>
    /// Command handler for adding Vibe.UI components
    /// </summary>
    internal sealed class AddComponentCommand
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("a1234567-b890-1234-5678-901234567890");
        private readonly AsyncPackage package;

        private AddComponentCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static AddComponentCommand Instance { get; private set; }

        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider => this.package;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new AddComponentCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var components = new[]
            {
                "button", "checkbox", "input", "select", "card", "dialog", "toast",
                "badge", "label", "togglegroup", "radiogroup", "tabs", "accordion"
            };

            var dialog = new ComponentSelectionDialog(components);
            if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(dialog.SelectedComponent))
            {
                AddComponent(dialog.SelectedComponent);
            }
        }

        private void AddComponent(string componentName)
        {
            try
            {
                var projectDir = GetProjectDirectory();
                if (string.IsNullOrEmpty(projectDir))
                {
                    MessageBox.Show("Could not determine project directory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "vibe",
                        Arguments = $"add {componentName} -y",
                        WorkingDirectory = projectDir,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    MessageBox.Show($"{componentName} component added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var error = process.StandardError.ReadToEnd();
                    MessageBox.Show($"Failed to add component: {error}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetProjectDirectory()
        {
            // This would use DTE to get the current project directory
            // For now, return current directory
            return Directory.GetCurrentDirectory();
        }
    }

    public class ComponentSelectionDialog : Form
    {
        public string SelectedComponent { get; private set; }

        public ComponentSelectionDialog(string[] components)
        {
            InitializeComponent(components);
        }

        private void InitializeComponent(string[] components)
        {
            this.Text = "Select Component";
            this.Size = new System.Drawing.Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;

            var listBox = new ListBox
            {
                Dock = DockStyle.Fill,
                Items = { }
            };

            foreach (var component in components)
            {
                listBox.Items.Add(component);
            }

            var okButton = new System.Windows.Forms.Button
            {
                Text = "Add",
                DialogResult = DialogResult.OK,
                Dock = DockStyle.Bottom
            };

            okButton.Click += (s, e) =>
            {
                if (listBox.SelectedItem != null)
                {
                    SelectedComponent = listBox.SelectedItem.ToString();
                }
            };

            this.Controls.Add(listBox);
            this.Controls.Add(okButton);
            this.AcceptButton = okButton;
        }
    }
}
