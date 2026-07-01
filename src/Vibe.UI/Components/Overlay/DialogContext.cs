namespace Vibe.UI.Components;

/// <summary>
/// Provides cascading context for Dialog components to communicate state and actions.
/// This enables compositional patterns where DialogTrigger and DialogClose can control the dialog state.
/// </summary>
public class DialogContext
{
    private readonly Func<Task> _openAction;
    private readonly Func<Task> _closeAction;
    private readonly Action? _onStateChanged;

    /// <summary>
    /// Gets the element ID used to label the dialog title.
    /// </summary>
    public string? TitleId { get; private set; }

    /// <summary>
    /// Gets the element ID used to describe the dialog body.
    /// </summary>
    public string? DescriptionId { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogContext"/> class.
    /// </summary>
    /// <param name="openAction">The action to execute when opening the dialog.</param>
    /// <param name="closeAction">The action to execute when closing the dialog.</param>
    /// <param name="onStateChanged">An optional callback to notify the owner that context state changed.</param>
    public DialogContext(Func<Task> openAction, Func<Task> closeAction, Action? onStateChanged = null)
    {
        _openAction = openAction;
        _closeAction = closeAction;
        _onStateChanged = onStateChanged;
    }

    /// <summary>
    /// Opens the dialog.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task Open() => _openAction();

    /// <summary>
    /// Closes the dialog.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task Close() => _closeAction();

    /// <summary>
    /// Updates the title element ID.
    /// </summary>
    /// <param name="id">The title element ID, or <see langword="null"/> when no title is registered.</param>
    public void SetTitleId(string? id)
    {
        if (TitleId == id)
            return;

        TitleId = id;
        _onStateChanged?.Invoke();
    }

    /// <summary>
    /// Updates the description element ID.
    /// </summary>
    /// <param name="id">The description element ID, or <see langword="null"/> when no description is registered.</param>
    public void SetDescriptionId(string? id)
    {
        if (DescriptionId == id)
            return;

        DescriptionId = id;
        _onStateChanged?.Invoke();
    }
}
