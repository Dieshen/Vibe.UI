using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Vibe.UI.Services.Dialog
{
    /// <summary>
    /// Service for displaying modal dialogs.
    /// </summary>
    public class DialogService : IDialogService
    {
        private readonly object _sync = new();
        private TaskCompletionSource<object?>? _dialogResult;
        private string? _currentDialogId;

        /// <summary>
        /// Event raised when a dialog is opened.
        /// </summary>
        public event EventHandler<DialogOpenedEventArgs>? OnDialogOpened;

        /// <summary>
        /// Event raised when a dialog is closed.
        /// </summary>
        public event EventHandler<DialogClosedEventArgs>? OnDialogClosed;

        /// <summary>
        /// Shows a simple alert dialog with an OK button.
        /// </summary>
        public Task ShowAlertAsync(string title, string message, string okText = "OK")
        {
            var content = new RenderFragment(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "vibe-dialog-content");
                
                builder.OpenElement(2, "div");
                builder.AddAttribute(3, "class", "vibe-dialog-message");
                builder.AddContent(4, message);
                builder.CloseElement(); // message div
                
                builder.OpenElement(5, "div");
                builder.AddAttribute(6, "class", "vibe-dialog-actions");
                
                builder.OpenElement(7, "button");
                builder.AddAttribute(8, "class", "vibe-dialog-button vibe-dialog-button-primary");
                builder.AddAttribute(9, "onclick", EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, () => Close(true)));
                builder.AddContent(10, okText);
                builder.CloseElement(); // button
                
                builder.CloseElement(); // actions div
                
                builder.CloseElement(); // content div
            });

            return ShowCustomAsync(title, content);
        }

        /// <summary>
        /// Shows a confirmation dialog with OK and Cancel buttons.
        /// </summary>
        public async Task<bool> ShowConfirmAsync(string title, string message, string okText = "OK", string cancelText = "Cancel")
        {
            var content = new RenderFragment(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "vibe-dialog-content");
                
                builder.OpenElement(2, "div");
                builder.AddAttribute(3, "class", "vibe-dialog-message");
                builder.AddContent(4, message);
                builder.CloseElement(); // message div
                
                builder.OpenElement(5, "div");
                builder.AddAttribute(6, "class", "vibe-dialog-actions");
                
                builder.OpenElement(7, "button");
                builder.AddAttribute(8, "class", "vibe-dialog-button vibe-dialog-button-secondary");
                builder.AddAttribute(9, "onclick", EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, () => Close(false)));
                builder.AddContent(10, cancelText);
                builder.CloseElement(); // button
                
                builder.OpenElement(11, "button");
                builder.AddAttribute(12, "class", "vibe-dialog-button vibe-dialog-button-primary");
                builder.AddAttribute(13, "onclick", EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, () => Close(true)));
                builder.AddContent(14, okText);
                builder.CloseElement(); // button
                
                builder.CloseElement(); // actions div
                
                builder.CloseElement(); // content div
            });

            var result = await ShowCustomAsync(title, content);
            return result is bool boolResult && boolResult;
        }

        /// <summary>
        /// Shows a prompt dialog with a text input.
        /// </summary>
        public async Task<string> ShowPromptAsync(string title, string message, string defaultValue = "", string okText = "OK", string cancelText = "Cancel")
        {
            var inputId = "dialog-prompt-" + Guid.NewGuid().ToString("N");
            var inputValue = defaultValue;

            var content = new RenderFragment(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "vibe-dialog-content");
                
                builder.OpenElement(2, "div");
                builder.AddAttribute(3, "class", "vibe-dialog-message");
                builder.AddContent(4, message);
                builder.CloseElement(); // message div
                
                builder.OpenElement(5, "div");
                builder.AddAttribute(6, "class", "vibe-dialog-form");
                
                builder.OpenElement(7, "input");
                builder.AddAttribute(8, "id", inputId);
                builder.AddAttribute(9, "class", "vibe-dialog-input");
                builder.AddAttribute(10, "value", inputValue);
                builder.AddAttribute(11, "oninput", EventCallback.Factory.Create<Microsoft.AspNetCore.Components.ChangeEventArgs>(this, e => inputValue = e.Value?.ToString()));
                builder.CloseElement(); // input
                
                builder.CloseElement(); // form div
                
                builder.OpenElement(12, "div");
                builder.AddAttribute(13, "class", "vibe-dialog-actions");
                
                builder.OpenElement(14, "button");
                builder.AddAttribute(15, "class", "vibe-dialog-button vibe-dialog-button-secondary");
                builder.AddAttribute(16, "onclick", EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, () => Close(null)));
                builder.AddContent(17, cancelText);
                builder.CloseElement(); // button
                
                builder.OpenElement(18, "button");
                builder.AddAttribute(19, "class", "vibe-dialog-button vibe-dialog-button-primary");
                builder.AddAttribute(20, "onclick", EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, () => Close(inputValue)));
                builder.AddContent(21, okText);
                builder.CloseElement(); // button
                
                builder.CloseElement(); // actions div
                
                builder.CloseElement(); // content div
            });

            var result = await ShowCustomAsync(title, content);
            return result as string ?? string.Empty;
        }

        /// <summary>
        /// Shows a custom dialog using the specified component type.
        /// </summary>
        public Task<object?> ShowCustomAsync(string title, Type componentType, DialogParameters? parameters = null)
        {
            var content = new RenderFragment(builder =>
            {
                builder.OpenComponent(0, componentType);
                
                if (parameters != null)
                {
                    builder.AddMultipleAttributes(1, parameters.GetAll());
                }
                
                builder.CloseComponent();
            });

            return ShowCustomAsync(title, content);
        }

        /// <summary>
        /// Shows a custom dialog using the specified render fragment.
        /// </summary>
        public Task<object?> ShowCustomAsync(string title, RenderFragment content)
        {
            DialogOpenedEventArgs args;
            Task<object?> task;

            lock (_sync)
            {
                if (_dialogResult != null)
                {
                    throw new InvalidOperationException("A dialog is already open. Close it before opening another.");
                }

                _dialogResult = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);

                var dialogId = Guid.NewGuid().ToString();
                _currentDialogId = dialogId;

                args = new DialogOpenedEventArgs
                {
                    Id = dialogId,
                    Title = title,
                    Content = content
                };

                task = _dialogResult.Task;
            }

            OnDialogOpened?.Invoke(this, args);

            return task;
        }

        /// <summary>
        /// Closes the currently open dialog with the specified result.
        /// </summary>
        public void Close(object? result = null)
        {
            DialogClosedEventArgs? args = null;
            TaskCompletionSource<object?>? tcs = null;

            lock (_sync)
            {
                if (_dialogResult == null || _currentDialogId == null)
                {
                    return;
                }

                args = new DialogClosedEventArgs
                {
                    Id = _currentDialogId,
                    Result = result
                };

                tcs = _dialogResult;
                _dialogResult = null;
                _currentDialogId = null;
            }

            OnDialogClosed?.Invoke(this, args);
            tcs.TrySetResult(result);
        }
    }
}
