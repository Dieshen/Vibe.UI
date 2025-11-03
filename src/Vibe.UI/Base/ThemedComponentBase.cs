using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vibe.UI.Themes.Services;

namespace Vibe.UI.Base
{
    /// <summary>
    /// Base class for all theme-aware components
    /// </summary>
    public abstract class ThemedComponentBase : ComponentBase, IDisposable
    {
        private bool _isDisposed;

        /// <summary>
        /// Gets or sets the theme manager.
        /// </summary>
        [Inject]
        protected ThemeManager ThemeManager { get; set; }

        /// <summary>
        /// Gets or sets the CSS class for the component.
        /// </summary>
        [Parameter]
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets additional attributes for the component.
        /// </summary>
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> AdditionalAttributes { get; set; }

        /// <summary>
        /// Gets the combined CSS class including theme-specific classes.
        /// </summary>
        protected string CombinedClass => GetCombinedClass();

        /// <summary>
        /// Gets the component-specific CSS class.
        /// </summary>
        protected abstract string ComponentClass { get; }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            
            if (ThemeManager != null)
            {
                ThemeManager.ThemeChanged += OnThemeChanged;
            }
        }

        /// <inheritdoc/>
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (ThemeManager != null && !ThemeManager.CurrentTheme?.Variables.Any() == true)
            {
                await ThemeManager.InitializeAsync();
            }
        }

        /// <summary>
        /// Handles when the theme changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnThemeChanged(object sender, Themes.Services.ThemeChangedEventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Gets the combined CSS class.
        /// </summary>
        protected virtual string GetCombinedClass()
        {
            var classList = new List<string> { ComponentClass };

            if (!string.IsNullOrWhiteSpace(Class))
            {
                classList.Add(Class);
            }

            return string.Join(" ", classList.Where(c => !string.IsNullOrWhiteSpace(c)));
        }

        /// <summary>
        /// Combines multiple CSS classes into a single string, filtering out null or whitespace values.
        /// </summary>
        /// <param name="classes">The CSS classes to combine.</param>
        /// <returns>A space-separated string of non-null, non-whitespace CSS classes.</returns>
        protected static string CombineClasses(params string?[] classes)
        {
            return string.Join(" ", classes.Where(c => !string.IsNullOrWhiteSpace(c)));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the component.
        /// </summary>
        /// <param name="disposing">Whether this is being called from Dispose or the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                if (ThemeManager != null)
                {
                    ThemeManager.ThemeChanged -= OnThemeChanged;
                }
            }

            _isDisposed = true;
        }
    }
}