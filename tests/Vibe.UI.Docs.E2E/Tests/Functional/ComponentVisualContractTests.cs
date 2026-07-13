using Microsoft.Playwright;
using Shouldly;
using Vibe.UI.Docs.E2E.Infrastructure;
using Xunit;

namespace Vibe.UI.Docs.E2E.Tests.Functional;

[Trait("Category", TestCategories.Functional)]
public class ComponentVisualContractTests : E2ETestBase
{
    [Fact]
    public async Task CheckboxPreviewSupportsMixedAndPointerStates()
    {
        await NavigateAndWaitForBlazorAsync("/components/checkbox");

        var mixed = Page.GetByLabel("Partially selected", new() { Exact = true });
        (await mixed.GetAttributeAsync("data-indeterminate")).ShouldBe("true");
        (await mixed.Locator("xpath=following-sibling::*[1]").Locator("svg").CountAsync()).ShouldBe(1);

        await mixed.ClickAsync();
        (await mixed.GetAttributeAsync("aria-checked")).ShouldBe("true");
        (await mixed.GetAttributeAsync("data-indeterminate")).ShouldBe("false");

        var subscribe = Page.GetByLabel("Subscribe to newsletter").First;
        await subscribe.ClickAsync();
        (await subscribe.GetAttributeAsync("aria-checked")).ShouldBe("true");
        await Page.GetByText("Subscribed", new() { Exact = true }).WaitForAsync();
    }

    [Fact]
    public async Task SwitchPreviewHasStableSizesAndInteractiveFeedback()
    {
        await NavigateAndWaitForBlazorAsync("/components/switch");

        var switches = Page.Locator("input[role='switch']");
        (await switches.CountAsync()).ShouldBeGreaterThan(0);
        foreach (var input in await switches.AllAsync())
        {
            (await input.GetAttributeAsync("aria-label")).ShouldNotBeNullOrWhiteSpace();
        }

        var notifications = Page.GetByLabel("Enable notifications").First;
        await notifications.ClickAsync();
        (await notifications.GetAttributeAsync("aria-checked")).ShouldBe("true");
        await Page.GetByText("Notifications enabled", new() { Exact = true }).WaitForAsync();

        var smallWidth = await Page.Locator(".vibe-switch-sm").First.EvaluateAsync<double>("element => element.getBoundingClientRect().width");
        var defaultWidth = await Page.Locator(".vibe-switch:not(.vibe-switch-sm):not(.vibe-switch-lg)").First.EvaluateAsync<double>("element => element.getBoundingClientRect().width");
        var largeWidth = await Page.Locator(".vibe-switch-lg").First.EvaluateAsync<double>("element => element.getBoundingClientRect().width");

        smallWidth.ShouldBe(34);
        defaultWidth.ShouldBe(42);
        largeWidth.ShouldBe(50);
    }

    [Fact]
    public async Task SliderPreviewUpdatesSemanticValueWithoutDecorativeCheckerboard()
    {
        await NavigateAndWaitForBlazorAsync("/components/slider");

        var preview = Page.Locator(".preview-panel");
        var input = preview.Locator("input[type='range']");
        var output = preview.Locator("output");

        (await input.GetAttributeAsync("aria-label")).ShouldBe("Preview value");
        (await output.EvaluateAsync<string>("element => element.tagName")).ShouldBe("OUTPUT");
        (await preview.Locator(".vibe-slider-track").EvaluateAsync<double>("element => element.getBoundingClientRect().height")).ShouldBe(4);

        var hasCheckerboard = await Page.EvaluateAsync<bool>(
            "() => [...document.querySelectorAll('*')].some(element => getComputedStyle(element).backgroundImage.includes('conic-gradient'))");
        hasCheckerboard.ShouldBeFalse();

        await input.EvaluateAsync("element => { element.value = '80'; element.dispatchEvent(new Event('input', { bubbles: true })); }");
        await output.WaitForAsync();
        (await input.GetAttributeAsync("aria-valuenow")).ShouldBe("80");
        (await output.TextContentAsync()).ShouldBe("80");
    }

    [Fact]
    public async Task TreeViewPreviewUsesSharedIconsAndInteractiveStates()
    {
        await NavigateAndWaitForBlazorAsync("/components/treeview");

        var tree = Page.Locator(".preview-panel .vibe-tree-view");
        (await tree.Locator(".tree-node-icon svg:visible").CountAsync()).ShouldBe(4);
        var treeText = await tree.TextContentAsync();
        treeText.ShouldNotBeNull();
        treeText.ShouldNotContain("📁");

        var documents = tree.Locator(":scope > [role='treeitem']").First;
        await documents.GetByRole(AriaRole.Button, new() { Name = "Expand Documents" }).ClickAsync();
        (await documents.GetAttributeAsync("aria-expanded")).ShouldBe("true");
        await documents.GetByText("Work", new() { Exact = true }).WaitForAsync();

        await documents.Locator(":scope > .tree-node-content").ClickAsync();
        (await documents.GetAttributeAsync("aria-selected")).ShouldBe("true");
        var contentClass = await documents.Locator(":scope > .tree-node-content").GetAttributeAsync("class");
        contentClass.ShouldNotBeNull();
        contentClass.ShouldContain("selected");
    }

    [Fact]
    public async Task SpinnerLoadingButtonUsesDecorativeIndicator()
    {
        await NavigateAndWaitForBlazorAsync("/components/spinner");

        (await Page.Locator(".spinner-label").CountAsync()).ShouldBeGreaterThan(0);

        var button = Page.GetByRole(AriaRole.Button, new() { Name = "Saving changes", Exact = true });
        await button.WaitForAsync();
        var spinner = button.Locator(".vibe-spinner");
        (await spinner.GetAttributeAsync("aria-hidden")).ShouldBe("true");
        (await spinner.GetAttributeAsync("role")).ShouldBeNull();
        (await spinner.Locator(".sr-only").CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task VirtualScrollTracksNativeAndProgrammaticScrolling()
    {
        await NavigateAndWaitForBlazorAsync("/components/virtualscroll");

        var viewport = Page.Locator(".vibe-virtual-scroll").First;
        await viewport.GetByText("Item 1", new() { Exact = true }).WaitForAsync();

        await viewport.EvaluateAsync("element => { element.scrollTop = 4800; element.dispatchEvent(new Event('scroll')); }");
        await viewport.GetByText("Item 101", new() { Exact = true }).WaitForAsync();
        (await viewport.EvaluateAsync<double>("element => element.scrollTop")).ShouldBe(4800);

        await Page.GetByRole(AriaRole.Button, new() { Name = "Jump to item 501" }).ClickAsync();
        await viewport.GetByText("Item 501", new() { Exact = true }).WaitForAsync();
        (await viewport.EvaluateAsync<double>("element => element.scrollTop")).ShouldBe(24000);
        await Page.GetByText("Jumped to item 501.", new() { Exact = true }).WaitForAsync();
    }

    [Fact]
    public async Task TableKeepsMobileCellsReadableWhileOverflowingHorizontally()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/table");

        var table = Page.Locator(".vibe-table").First;
        var metrics = await table.EvaluateAsync<TableMetrics>(
            "element => ({ clientWidth: element.clientWidth, scrollWidth: element.scrollWidth, cellWhiteSpace: getComputedStyle(element.querySelector('td')).whiteSpace })");

        metrics.ScrollWidth.ShouldBeGreaterThan(metrics.ClientWidth);
        metrics.CellWhiteSpace.ShouldBe("nowrap");
        (await table.GetByText("John Doe", new() { Exact = true }).CountAsync()).ShouldBe(1);
    }

    [Fact]
    public async Task ColorPickerFitsMobileViewportAndPreservesKeyboardFocus()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/colorpicker");

        var trigger = Page.Locator(".vibe-color-picker-preview").First;
        await trigger.FocusAsync();
        await trigger.PressAsync("Enter");

        var dialog = Page.GetByRole(AriaRole.Dialog, new() { Name = "Color picker", Exact = true });
        await dialog.WaitForAsync();
        (await Page.EvaluateAsync<string>("() => document.activeElement?.className ?? ''"))
            .ShouldContain("vibe-color-picker-hue");

        var popoverBox = await dialog.BoundingBoxAsync();
        var saturationBox = await dialog.Locator(".vibe-color-picker-saturation").BoundingBoxAsync();
        popoverBox.ShouldNotBeNull();
        saturationBox.ShouldNotBeNull();
        popoverBox.X.ShouldBeGreaterThanOrEqualTo(0);
        popoverBox.Y.ShouldBeGreaterThanOrEqualTo(0);
        (popoverBox.X + popoverBox.Width).ShouldBeLessThanOrEqualTo(390);
        (popoverBox.Y + popoverBox.Height).ShouldBeLessThanOrEqualTo(844);
        saturationBox.Width.ShouldBe(240);
        saturationBox.Height.ShouldBe(144);

        await dialog.GetByRole(AriaRole.Spinbutton, new() { Name = "R", Exact = true }).FillAsync("255");
        (await trigger.GetAttributeAsync("aria-label")).ShouldBe("Color picker, selected #FFB8A6");
        (await dialog.Locator("svg:not(.vibe-icon)").CountAsync()).ShouldBe(0);

        await Page.Keyboard.PressAsync("Escape");
        (await dialog.CountAsync()).ShouldBe(0);
        (await trigger.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();
    }

    [Fact]
    public async Task FileUploadProcessesDroppedFilesAndUsesSharedIcons()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/fileupload");

        var upload = Page.Locator(".vibe-file-upload").First;
        await upload.EvaluateAsync(
            """
            element => {
                const transfer = new DataTransfer();
                transfer.items.add(new File(['hello from Vibe.UI'], 'notes.txt', { type: 'text/plain' }));
                element.dispatchEvent(new DragEvent('dragenter', { bubbles: true, cancelable: true, dataTransfer: transfer }));
            }
            """);
        (await upload.GetAttributeAsync("class") ?? string.Empty).ShouldContain("file-upload-dragging");

        await upload.EvaluateAsync(
            """
            element => {
                const transfer = new DataTransfer();
                transfer.items.add(new File(['hello from Vibe.UI'], 'notes.txt', { type: 'text/plain' }));
                element.dispatchEvent(new DragEvent('drop', { bubbles: true, cancelable: true, dataTransfer: transfer }));
            }
            """);

        await upload.GetByText("notes.txt", new() { Exact = true }).WaitForAsync();
        (await upload.GetAttributeAsync("class") ?? string.Empty).ShouldNotContain("file-upload-dragging");
        (await upload.GetByText("18 B", new() { Exact = true }).CountAsync()).ShouldBe(1);
        (await upload.Locator("svg.vibe-icon").CountAsync()).ShouldBeGreaterThanOrEqualTo(2);
        var uploadText = await upload.TextContentAsync() ?? string.Empty;
        uploadText.ShouldNotContain("\U0001F4C1");
        uploadText.ShouldNotContain("\U0001F4C4");

        await upload.GetByRole(AriaRole.Button, new() { Name = "Remove notes.txt", Exact = true }).ClickAsync();
        await upload.GetByText("Drop files here or click to browse", new() { Exact = true }).WaitForAsync();
        (await upload.GetByText("notes.txt", new() { Exact = true }).CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task KanbanMovesCardsByPointerAndRetainsFocusForKeyboardMoves()
    {
        await Page.SetViewportSizeAsync(1280, 900);
        await NavigateAndWaitForBlazorAsync("/components/kanbanboard");

        var source = Page.Locator(".kanban-card[aria-label='Map component inventory']");
        var destination = Page.Locator(".kanban-column").Nth(1).Locator(".kanban-cards");
        await source.DragToAsync(destination);

        (await destination.Locator(".kanban-card[aria-label='Map component inventory']").CountAsync()).ShouldBe(1);
        await Page.GetByText(
            "Moved Map component inventory from backlog to active.",
            new() { Exact = true }).WaitForAsync();

        await NavigateAndWaitForBlazorAsync("/components/kanbanboard");
        source = Page.Locator(".kanban-card[aria-label='Map component inventory']");
        await source.FocusAsync();
        await source.PressAsync("Control+ArrowRight");

        destination = Page.Locator(".kanban-column").Nth(1).Locator(".kanban-cards");
        (await destination.Locator(".kanban-card[aria-label='Map component inventory']").CountAsync()).ShouldBe(1);
        (await source.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();
    }

    [Fact]
    public async Task SheetTrapsFocusLocksScrollAndRestoresMobileOpener()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/sheet");

        var opener = Page.GetByRole(AriaRole.Button, new() { Name = "Open right sheet", Exact = true });
        await opener.ClickAsync();

        var dialog = Page.GetByRole(AriaRole.Dialog, new() { Name = "Edit project", Exact = true });
        await dialog.WaitForAsync();
        await Page.WaitForTimeoutAsync(350);
        var box = await dialog.BoundingBoxAsync();
        box.ShouldNotBeNull();
        box.X.ShouldBeGreaterThan(0);
        box.Width.ShouldBeInRange(342, 344);
        (box.X + box.Width).ShouldBeLessThanOrEqualTo(390);
        (await Page.EvaluateAsync<string>("() => document.body.style.overflow")).ShouldBe("hidden");
        (await Page.EvaluateAsync<string>("() => document.activeElement?.getAttribute('aria-label') ?? ''")).ShouldBe("Close");
        (await dialog.Locator("svg:not(.vibe-icon)").CountAsync()).ShouldBe(0);

        await Page.Keyboard.PressAsync("Shift+Tab");
        (await Page.EvaluateAsync<string>("() => document.activeElement?.textContent?.trim() ?? ''")).ShouldBe("Save changes");

        await Page.Keyboard.PressAsync("Escape");
        await dialog.WaitForAsync(new() { State = WaitForSelectorState.Detached });
        (await Page.EvaluateAsync<string>("() => document.body.style.overflow")).ShouldBe(string.Empty);
        (await opener.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();
    }

    [Fact]
    public async Task SplitterSupportsPointerAndKeyboardResizeWithoutOverflow()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/splitter");

        var splitter = Page.Locator(".vibe-splitter").First;
        var divider = splitter.Locator(".splitter-divider");
        var rootBox = await splitter.BoundingBoxAsync();
        var dividerBox = await divider.BoundingBoxAsync();
        rootBox.ShouldNotBeNull();
        dividerBox.ShouldNotBeNull();

        await Page.Mouse.MoveAsync(dividerBox.X + dividerBox.Width / 2, dividerBox.Y + dividerBox.Height / 2);
        await Page.Mouse.DownAsync();
        await Page.Mouse.MoveAsync(
            (float)(rootBox.X + rootBox.Width * 0.6),
            dividerBox.Y + dividerBox.Height / 2,
            new() { Steps = 8 });
        await Page.Mouse.UpAsync();

        var pointerSize = double.Parse(
            await divider.GetAttributeAsync("aria-valuenow") ?? "0",
            System.Globalization.CultureInfo.InvariantCulture);
        pointerSize.ShouldBeInRange(59.5, 60.5);
        (await divider.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();
        (await splitter.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth")).ShouldBeTrue();
        (await splitter.TextContentAsync() ?? string.Empty).ShouldNotContain("\u22EE");

        await divider.PressAsync("ArrowRight");
        var keyboardSize = double.Parse(
            await divider.GetAttributeAsync("aria-valuenow") ?? "0",
            System.Globalization.CultureInfo.InvariantCulture);
        keyboardSize.ShouldBe(pointerSize + 1);
        await Page.GetByText($"First pane: {keyboardSize:0.#}%", new() { Exact = true }).WaitForAsync();
    }

    [Fact]
    public async Task ResizableUsesRootRelativePointerAndKeyboardSizing()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/resizable");

        var root = Page.Locator(".vibe-resizable").First;
        var handle = root.Locator(".resizable-handle");
        var rootBox = await root.BoundingBoxAsync();
        var handleBox = await handle.BoundingBoxAsync();
        rootBox.ShouldNotBeNull();
        handleBox.ShouldNotBeNull();

        await Page.Mouse.MoveAsync(handleBox.X + 2, handleBox.Y + handleBox.Height / 2);
        await Page.Mouse.DownAsync();
        await Page.Mouse.MoveAsync(rootBox.X + 310, handleBox.Y + handleBox.Height / 2, new() { Steps = 8 });
        await Page.Mouse.UpAsync();
        await Page.WaitForTimeoutAsync(220);

        (await root.EvaluateAsync<double>("element => element.getBoundingClientRect().width")).ShouldBe(310);
        (await handle.GetAttributeAsync("aria-valuenow")).ShouldBe("310");
        (await handle.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();
        (await Page.EvaluateAsync<string>("() => document.body.style.cursor")).ShouldBe(string.Empty);
        (await handle.Locator("svg.vibe-icon").CountAsync()).ShouldBe(1);

        await handle.PressAsync("ArrowLeft");
        (await root.EvaluateAsync<double>("element => element.getBoundingClientRect().width")).ShouldBe(300);
        (await handle.GetAttributeAsync("aria-valuenow")).ShouldBe("300");
        await Page.GetByText("Panel: 300px", new() { Exact = true }).WaitForAsync();
    }

    [Fact]
    public async Task RatingUsesSingleSelectionRovingFocusAndSharedIcons()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/rating");

        var rating = Page.Locator(".vibe-rating").First;
        var stars = rating.GetByRole(AriaRole.Radio);
        (await stars.CountAsync()).ShouldBe(5);
        (await rating.Locator("[role='radio'][aria-checked='true']").CountAsync()).ShouldBe(1);
        (await rating.Locator("svg.vibe-icon").CountAsync()).ShouldBe(6);
        (await rating.Locator(".star-half svg.vibe-icon").CountAsync()).ShouldBe(2);

        foreach (var star in await stars.AllAsync())
        {
            var box = await star.BoundingBoxAsync();
            box.ShouldNotBeNull();
            box.Width.ShouldBeGreaterThanOrEqualTo(39.5f);
            box.Height.ShouldBeGreaterThanOrEqualTo(39.5f);
        }

        var ratingText = await rating.TextContentAsync() ?? string.Empty;
        ratingText.ShouldNotContain("★");
        ratingText.ShouldNotContain("☆");

        await stars.Nth(3).FocusAsync();
        await stars.Nth(3).PressAsync("ArrowRight");
        await Page.GetByText("Selected rating: 5", new() { Exact = true }).WaitForAsync();
        (await rating.GetByRole(AriaRole.Radio, new() { Checked = true }).GetAttributeAsync("aria-label"))
            .ShouldBe("5 stars, selected");
        await Page.WaitForFunctionAsync(
            "() => document.activeElement?.getAttribute('aria-label') === '5 stars, selected'");
        (await stars.Nth(4).EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();
        (await rating.Locator("[role='radio'][aria-checked='true']").CountAsync()).ShouldBe(1);

        await stars.Nth(4).ClickAsync();
        await Page.GetByText("Selected rating: 4.5", new() { Exact = true }).WaitForAsync();
        (await rating.GetByRole(AriaRole.Radio, new() { Checked = true }).GetAttributeAsync("aria-label"))
            .ShouldBe("4.5 stars, selected");
    }

    [Fact]
    public async Task ImageCropperSupportsPointerResizeAndProducesPngOutput()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/imagecropper");

        var cropper = Page.Locator(".vibe-image-cropper").First;
        var cropBox = cropper.Locator(".crop-box");
        (await cropBox.Locator("[data-crop-handle]").CountAsync()).ShouldBe(8);
        (await cropper.Locator(".cropper-controls svg.vibe-icon").CountAsync()).ShouldBe(6);
        (await cropper.Locator(".cropper-canvas").EvaluateAsync<string>("element => getComputedStyle(element).backgroundImage"))
            .ShouldBe("none");

        foreach (var control in await cropper.Locator(".control-btn").AllAsync())
        {
            var box = await control.BoundingBoxAsync();
            box.ShouldNotBeNull();
            box.Height.ShouldBeGreaterThanOrEqualTo(40);
        }

        var initialBox = await cropBox.BoundingBoxAsync();
        initialBox.ShouldNotBeNull();
        await Page.Mouse.MoveAsync(initialBox.X + initialBox.Width / 2, initialBox.Y + initialBox.Height / 2);
        await Page.Mouse.DownAsync();
        await Page.Mouse.MoveAsync(
            initialBox.X + initialBox.Width / 2 + 24,
            initialBox.Y + initialBox.Height / 2 + 18,
            new() { Steps = 8 });
        await Page.Mouse.UpAsync();
        await Page.WaitForTimeoutAsync(220);

        var movedBox = await cropBox.BoundingBoxAsync();
        movedBox.ShouldNotBeNull();
        movedBox.X.ShouldBeGreaterThan(initialBox.X + 20);
        movedBox.Y.ShouldBeGreaterThan(initialBox.Y + 14);
        (await cropBox.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();

        var southeast = cropBox.Locator("[data-crop-handle='se']");
        var handleBox = await southeast.BoundingBoxAsync();
        handleBox.ShouldNotBeNull();
        await Page.Mouse.MoveAsync(handleBox.X + handleBox.Width / 2, handleBox.Y + handleBox.Height / 2);
        await Page.Mouse.DownAsync();
        await Page.Mouse.MoveAsync(
            handleBox.X + handleBox.Width / 2 - 40,
            handleBox.Y + handleBox.Height / 2 - 30,
            new() { Steps = 8 });
        await Page.Mouse.UpAsync();
        await Page.WaitForTimeoutAsync(220);

        var resizedBox = await cropBox.BoundingBoxAsync();
        resizedBox.ShouldNotBeNull();
        resizedBox.Width.ShouldBeLessThan(movedBox.Width - 35);
        resizedBox.Height.ShouldBeLessThan(movedBox.Height - 25);
        (await Page.EvaluateAsync<string>("() => document.body.style.cursor")).ShouldBe(string.Empty);

        await cropper.GetByRole(AriaRole.Button, new() { Name = "Rotate right", Exact = true }).ClickAsync();
        await cropper.GetByRole(AriaRole.Button, new() { Name = "Crop image", Exact = true }).ClickAsync();
        var result = Page.GetByRole(AriaRole.Img, new() { Name = "Cropped Vibe.UI wave mark", Exact = true });
        await result.WaitForAsync();
        (await result.GetAttributeAsync("src") ?? string.Empty).ShouldStartWith("data:image/png;base64,");

        var pixels = await result.EvaluateAsync<int>(
            """
            async image => {
                await image.decode();
                const canvas = document.createElement('canvas');
                canvas.width = image.naturalWidth;
                canvas.height = image.naturalHeight;
                const context = canvas.getContext('2d');
                context.drawImage(image, 0, 0);
                const data = context.getImageData(0, 0, canvas.width, canvas.height).data;
                let count = 0;
                for (let index = 3; index < data.length; index += 64) {
                    if (data[index] > 0) count++;
                }
                return count;
            }
            """);
        pixels.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task AlertDialogRequiresExplicitBackdropActionAndRestoresFocus()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/alertdialog");

        var opener = Page.GetByRole(AriaRole.Button, new() { Name = "Delete project", Exact = true }).First;
        await opener.ClickAsync();
        var dialog = Page.GetByRole(AriaRole.Alertdialog);
        await dialog.WaitForAsync();
        await Page.WaitForTimeoutAsync(120);

        var contentBox = await dialog.Locator(".vibe-alert-dialog-content").BoundingBoxAsync();
        contentBox.ShouldNotBeNull();
        contentBox.X.ShouldBeInRange(16, 18);
        contentBox.Width.ShouldBeInRange(354, 358);
        (await Page.EvaluateAsync<string>("() => document.body.style.overflow")).ShouldBe("hidden");
        (await Page.EvaluateAsync<string>("() => document.activeElement?.textContent?.trim() ?? ''")).ShouldBe("Cancel");
        (await dialog.GetByRole(AriaRole.Button, new() { Name = "Close", Exact = true }).CountAsync()).ShouldBe(0);

        await dialog.Locator(".vibe-alert-dialog-backdrop").ClickAsync(new() { Position = new() { X = 4, Y = 4 } });
        (await dialog.CountAsync()).ShouldBe(1);

        var footerButtons = dialog.Locator(".vibe-alert-dialog-footer button");
        foreach (var button in await footerButtons.AllAsync())
        {
            var box = await button.BoundingBoxAsync();
            box.ShouldNotBeNull();
            box.Width.ShouldBeGreaterThanOrEqualTo(320);
        }

        await Page.Keyboard.PressAsync("Shift+Tab");
        (await Page.EvaluateAsync<string>("() => document.activeElement?.textContent?.trim() ?? ''"))
            .ShouldBe("Delete project");
        await Page.Keyboard.PressAsync("Escape");
        await dialog.WaitForAsync(new() { State = WaitForSelectorState.Detached });
        await Page.WaitForFunctionAsync(
            "() => document.activeElement?.textContent?.trim() === 'Delete project'");
        (await opener.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();
        (await Page.EvaluateAsync<string>("() => document.body.style.overflow")).ShouldBe(string.Empty);
    }

    [Fact]
    public async Task CarouselSupportsPointerKeyboardAndAutoplayPause()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/carousel");

        var carousel = Page.GetByRole(AriaRole.Region, new() { Name = "Vibe.UI release highlights", Exact = true });
        var viewport = carousel.Locator(".carousel-viewport");
        var pause = carousel.Locator(".carousel-autoplay-button");

        (await carousel.Locator("svg.vibe-icon").CountAsync()).ShouldBe(3);
        (await carousel.Locator("svg:not(.vibe-icon)").CountAsync()).ShouldBe(0);
        (await viewport.GetAttributeAsync("aria-label")).ShouldBe("Slide 1 of 3");
        (await pause.GetAttributeAsync("aria-label")).ShouldBe("Pause automatic slide rotation");

        foreach (var button in await carousel.Locator(".carousel-button").AllAsync())
        {
            var box = await button.BoundingBoxAsync();
            box.ShouldNotBeNull();
            box.Width.ShouldBeGreaterThanOrEqualTo(40);
            box.Height.ShouldBeGreaterThanOrEqualTo(40);
        }

        foreach (var indicator in await carousel.Locator(".carousel-indicator").AllAsync())
        {
            var box = await indicator.BoundingBoxAsync();
            box.ShouldNotBeNull();
            box.Width.ShouldBeGreaterThanOrEqualTo(32);
            box.Height.ShouldBeGreaterThanOrEqualTo(32);
        }

        await pause.ClickAsync();
        (await pause.GetAttributeAsync("aria-pressed")).ShouldBe("true");
        await carousel.GetByRole(AriaRole.Button, new() { Name = "Resume automatic slide rotation", Exact = true }).WaitForAsync();

        var viewportBox = await viewport.BoundingBoxAsync();
        viewportBox.ShouldNotBeNull();
        await Page.Mouse.MoveAsync(viewportBox.X + viewportBox.Width * 0.75f, viewportBox.Y + viewportBox.Height / 2);
        await Page.Mouse.DownAsync();
        await Page.Mouse.MoveAsync(
            viewportBox.X + viewportBox.Width * 0.25f,
            viewportBox.Y + viewportBox.Height / 2,
            new() { Steps = 8 });
        await Page.Mouse.UpAsync();
        await Page.GetByText("Slide 2 of 3", new() { Exact = true }).Last.WaitForAsync();

        await carousel.FocusAsync();
        await Page.Keyboard.PressAsync("ArrowRight");
        await Page.GetByText("Slide 3 of 3", new() { Exact = true }).Last.WaitForAsync();
        (await viewport.GetAttributeAsync("aria-label")).ShouldBe("Slide 3 of 3");
        (await carousel.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();
    }

    [Fact]
    public async Task FormFieldLabelsFocusControlsAndExposeValidationOnTheInput()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/formfield");

        var field = Page.Locator(".vibe-form-field").First;
        var label = field.Locator("label[for='work-email']");
        var input = field.Locator("#work-email");

        await label.ClickAsync();
        (await input.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();
        (await input.GetAttributeAsync("aria-describedby")).ShouldBe("work-email-description");
        (await input.GetAttributeAsync("aria-required")).ShouldBe("true");
        (await input.GetAttributeAsync("required")).ShouldNotBeNull();
        (await field.Locator(".form-field-required").GetAttributeAsync("aria-hidden")).ShouldBe("true");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Show error", Exact = true }).ClickAsync();
        (await input.GetAttributeAsync("aria-invalid")).ShouldBe("true");
        (await input.GetAttributeAsync("aria-describedby")).ShouldBe("work-email-description work-email-error");
        (await input.GetAttributeAsync("aria-errormessage")).ShouldBe("work-email-error");
        await field.GetByRole(AriaRole.Alert).GetByText("Enter a valid work email", new() { Exact = true }).WaitForAsync();

        await label.ClickAsync();
        var errorRing = await field.Locator(".vibe-input-wrapper").EvaluateAsync<string>(
            "element => getComputedStyle(element).boxShadow");
        errorRing.ShouldNotBe("none");
    }

    [Fact]
    public async Task TimelineUsesOneCenteredDesktopRailAndCollapsesOnMobile()
    {
        await Page.SetViewportSizeAsync(1280, 900);
        await NavigateAndWaitForBlazorAsync("/components/timeline");

        var timeline = Page.GetByRole(AriaRole.List, new() { Name = "Beta release history", Exact = true });
        var items = timeline.GetByRole(AriaRole.Listitem);
        (await items.CountAsync()).ShouldBe(3);
        (await timeline.Locator(".timeline-status").AllTextContentsAsync())
            .ShouldBe(new[] { "Info", "Success", "Warning" });

        var firstMarker = await items.Nth(0).Locator(".timeline-marker").BoundingBoxAsync();
        var secondMarker = await items.Nth(1).Locator(".timeline-marker").BoundingBoxAsync();
        var thirdMarker = await items.Nth(2).Locator(".timeline-marker").BoundingBoxAsync();
        firstMarker.ShouldNotBeNull();
        secondMarker.ShouldNotBeNull();
        thirdMarker.ShouldNotBeNull();
        Math.Abs(firstMarker.X - secondMarker.X).ShouldBeLessThan(1);
        Math.Abs(secondMarker.X - thirdMarker.X).ShouldBeLessThan(1);

        var firstContent = await items.Nth(0).Locator(".timeline-content").BoundingBoxAsync();
        var secondContent = await items.Nth(1).Locator(".timeline-content").BoundingBoxAsync();
        firstContent.ShouldNotBeNull();
        secondContent.ShouldNotBeNull();
        firstContent.X.ShouldBeGreaterThan(firstMarker.X + firstMarker.Width);
        (secondContent.X + secondContent.Width).ShouldBeLessThan(secondMarker.X);

        var firstConnector = await items.Nth(0).Locator(".timeline-connector").BoundingBoxAsync();
        firstConnector.ShouldNotBeNull();
        Math.Abs((firstConnector.Y + firstConnector.Height) - secondMarker.Y).ShouldBeLessThanOrEqualTo(2);

        await Page.SetViewportSizeAsync(390, 844);
        await Page.WaitForTimeoutAsync(120);
        foreach (var item in await items.AllAsync())
        {
            (await item.EvaluateAsync<string>("element => getComputedStyle(element).display")).ShouldBe("flex");
            var marker = await item.Locator(".timeline-marker").BoundingBoxAsync();
            var content = await item.Locator(".timeline-content").BoundingBoxAsync();
            marker.ShouldNotBeNull();
            content.ShouldNotBeNull();
            marker.X.ShouldBeLessThan(content.X);
            (await item.Locator(".timeline-content").EvaluateAsync<string>("element => getComputedStyle(element).textAlign"))
                .ShouldBe("left");
        }
    }

    [Fact]
    public async Task InputOtpDistributesBrowserInputAndWrapsWithinMobileWidth()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/inputotp");

        var otp = Page.GetByRole(AriaRole.Group, new() { Name = "Verification code", Exact = true });
        var slots = otp.Locator("input.input-otp-slot");
        (await slots.CountAsync()).ShouldBe(6);
        (await slots.Nth(0).GetAttributeAsync("maxlength")).ShouldBe("6");
        (await slots.Nth(0).GetAttributeAsync("autocomplete")).ShouldBe("one-time-code");
        (await slots.Nth(1).GetAttributeAsync("autocomplete")).ShouldBe("off");
        (await otp.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth")).ShouldBeTrue();

        await slots.Nth(0).FillAsync("123456");
        await Page.GetByText("Code accepted: 123456", new() { Exact = true }).WaitForAsync();
        for (var index = 0; index < 6; index++)
        {
            (await slots.Nth(index).InputValueAsync()).ShouldBe((index + 1).ToString());
        }

        await slots.Nth(3).FocusAsync();
        await slots.Nth(3).PressAsync("ArrowLeft");
        (await slots.Nth(2).EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();
    }

    [Fact]
    public async Task MentionsUsesAccessibleSuggestionsAndTouchSizedRemoval()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/mentions");

        var mentions = Page.Locator(".vibe-mentions").First;
        var input = mentions.GetByRole(AriaRole.Combobox, new() { Name = "Add contributors", Exact = true });
        await input.FillAsync("@gr");

        var suggestions = Page.GetByRole(AriaRole.Listbox, new() { Name = "Add contributors suggestions", Exact = true });
        await suggestions.WaitForAsync();
        var grace = suggestions.GetByRole(AriaRole.Option, new() { Name = "Grace Hopper", Exact = false });
        var suggestionBox = await grace.BoundingBoxAsync();
        suggestionBox.ShouldNotBeNull();
        suggestionBox.Height.ShouldBeGreaterThanOrEqualTo(40);
        (await input.GetAttributeAsync("aria-expanded")).ShouldBe("true");

        await grace.ClickAsync();
        await Page.GetByText("Added @Grace Hopper.", new() { Exact = true }).WaitForAsync();
        (await input.GetAttributeAsync("aria-expanded")).ShouldBe("false");

        var removeAda = mentions.GetByRole(AriaRole.Button, new() { Name = "Remove Ada Lovelace", Exact = true });
        var removeBox = await removeAda.BoundingBoxAsync();
        removeBox.ShouldNotBeNull();
        removeBox.Width.ShouldBeGreaterThanOrEqualTo(40);
        removeBox.Height.ShouldBeGreaterThanOrEqualTo(40);
        await removeAda.ClickAsync();
        await Page.GetByText("Removed @Ada Lovelace.", new() { Exact = true }).WaitForAsync();
    }

    [Fact]
    public async Task TransferListUsesRovingFocusAndMobileSizedTransferActions()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/transferlist");

        var transfer = Page.GetByRole(AriaRole.Group, new() { Name = "Assign people to the release team", Exact = true });
        (await transfer.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth")).ShouldBeTrue();
        var lists = transfer.GetByRole(AriaRole.Listbox);
        (await lists.CountAsync()).ShouldBe(2);
        var sourceOptions = lists.Nth(0).GetByRole(AriaRole.Option);
        (await sourceOptions.Nth(0).GetAttributeAsync("tabindex")).ShouldBe("0");
        (await sourceOptions.Nth(1).GetAttributeAsync("tabindex")).ShouldBe("-1");

        await sourceOptions.Nth(0).FocusAsync();
        await sourceOptions.Nth(0).PressAsync("ArrowDown");
        (await sourceOptions.Nth(1).EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();

        await sourceOptions.Nth(0).ClickAsync();
        var move = transfer.GetByRole(AriaRole.Button, new() { Name = "Move selected Available people items to Assigned people", Exact = true });
        var moveBox = await move.BoundingBoxAsync();
        moveBox.ShouldNotBeNull();
        moveBox.Width.ShouldBeGreaterThanOrEqualTo(40);
        moveBox.Height.ShouldBeGreaterThanOrEqualTo(40);
        await move.ClickAsync();
        await Page.GetByText("2 people assigned", new() { Exact = true }).WaitForAsync();
    }

    [Fact]
    public async Task ToggleGroupKeepsSelectionWhileRovingFocusOnMobile()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/togglegroup");

        var group = Page.GetByRole(AriaRole.Group, new() { Name = "Text alignment", Exact = true });
        (await group.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth")).ShouldBeTrue();
        var left = group.GetByRole(AriaRole.Button, new() { Name = "Left", Exact = true });
        var center = group.GetByRole(AriaRole.Button, new() { Name = "Center", Exact = true });
        var leftBox = await left.BoundingBoxAsync();
        leftBox.ShouldNotBeNull();
        leftBox.Height.ShouldBeGreaterThanOrEqualTo(40);
        (await left.GetAttributeAsync("aria-pressed")).ShouldBe("true");

        await left.FocusAsync();
        await left.PressAsync("ArrowRight");
        await Page.WaitForTimeoutAsync(120);
        (await center.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();
        (await left.GetAttributeAsync("aria-pressed")).ShouldBe("true");
        await center.ClickAsync();
        (await center.GetAttributeAsync("aria-pressed")).ShouldBe("true");
        await Page.GetByText("Alignment: center", new() { Exact = true }).WaitForAsync();
    }

    [Fact]
    public async Task ChartAppliesThemeTokensAndRendersAccessibleData()
    {
        var chartErrors = new List<string>();
        Page.Console += (_, message) =>
        {
            if (string.Equals(message.Type, "error", StringComparison.OrdinalIgnoreCase))
            {
                chartErrors.Add(message.Text);
            }
        };

        await Page.SetViewportSizeAsync(1280, 900);
        await NavigateAndWaitForBlazorAsync("/components/chart");

        var charts = Page.Locator(".vibe-chart");
        (await charts.CountAsync()).ShouldBeGreaterThanOrEqualTo(3);
        (await charts.Nth(0).Locator(".vibe-chart-accessible-data table").CountAsync()).ShouldBe(1);
        ((await charts.Nth(0).Locator(".vibe-chart-accessible-data").TextContentAsync()) ?? string.Empty)
            .ShouldContain("Monthly revenue has 6 data points across 2 series.");
        (await charts.Nth(2).Locator(".vibe-chart-legend-item").CountAsync()).ShouldBe(3);

        try
        {
            await Page.WaitForFunctionAsync(
                "() => Object.keys(window.vibeChart?.charts ?? {}).length >= 3");
        }
        catch (TimeoutException)
        {
            var runtime = await Page.EvaluateAsync<string>(
                "() => JSON.stringify({ chartConstructor: typeof window.Chart, vibeChart: typeof window.vibeChart, createChart: typeof window.vibeChart?.createChart, chartCount: Object.keys(window.vibeChart?.charts ?? {}).length, canvasCount: document.querySelectorAll('.vibe-chart-canvas').length })");

            throw new InvalidOperationException(
                $"Chart instances did not initialize. Runtime: {runtime}. Browser errors: {string.Join(" | ", chartErrors)}");
        }

        var canvas = charts.Nth(0).Locator(".vibe-chart-canvas");
        await Page.WaitForTimeoutAsync(500);
        var beforeCanvas = await canvas.EvaluateAsync<string>("element => element.toDataURL()");
        var beforeTheme = await Page.EvaluateAsync<string>(
            "() => getComputedStyle(document.documentElement).getPropertyValue('--vibe-muted-foreground').trim()");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Toggle theme", Exact = true }).ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        var afterCanvas = await canvas.EvaluateAsync<string>("element => element.toDataURL()");
        var afterTheme = await Page.EvaluateAsync<string>(
            "() => getComputedStyle(document.documentElement).getPropertyValue('--vibe-muted-foreground').trim()");

        afterTheme.ShouldNotBe(beforeTheme);
        afterCanvas.ShouldNotBe(beforeCanvas);
    }

    [Fact]
    public async Task DataTableSupportsResponsiveOverflowAndAccessibleFiltering()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/datatable");

        var dataTable = Page.Locator(".vibe-datatable").First;
        var overflowRegion = dataTable.Locator(".datatable-container");
        var table = overflowRegion.Locator(".datatable-table");

        (await overflowRegion.GetAttributeAsync("role")).ShouldBe("region");
        (await overflowRegion.GetAttributeAsync("tabindex")).ShouldBe("0");
        (await overflowRegion.GetAttributeAsync("aria-label")).ShouldBe("Product inventory, scrollable table");
        (await table.Locator("caption").TextContentAsync()).ShouldBe("Product inventory");
        (await table.GetAttributeAsync("aria-label")).ShouldBeNull();

        var regionWidth = await overflowRegion.EvaluateAsync<double>("element => element.clientWidth");
        var tableWidth = await table.EvaluateAsync<double>("element => element.scrollWidth");
        tableWidth.ShouldBeGreaterThan(regionWidth);

        await overflowRegion.FocusAsync();
        (await overflowRegion.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();

        var search = Page.GetByRole(AriaRole.Textbox, new() { Name = "Search products", Exact = true });
        await search.FillAsync("Mouse");
        await Page.GetByText("1 entry. Showing 1 to 1.", new() { Exact = true }).WaitForAsync();
        (await table.Locator("tbody tr").CountAsync()).ShouldBe(1);
        ((await table.Locator("tbody tr").First.TextContentAsync()) ?? string.Empty).ShouldContain("Wireless Mouse");

        await search.FillAsync(string.Empty);
        await Page.GetByText("8 entries. Showing 1 to 5.", new() { Exact = true }).WaitForAsync();

        var priceSort = table.Locator(".datatable-sort-button").Nth(1);
        await priceSort.ScrollIntoViewIfNeededAsync();
        (await overflowRegion.EvaluateAsync<double>("element => element.scrollLeft")).ShouldBeGreaterThan(0);
        await priceSort.ClickAsync();
        (await priceSort.GetAttributeAsync("aria-label")).ShouldBe("Sort by Price descending");
        (await table.Locator("thead th").Nth(1).GetAttributeAsync("aria-sort")).ShouldBe("ascending");
        ((await table.Locator("tbody tr").First.TextContentAsync()) ?? string.Empty).ShouldContain("Wireless Mouse");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Go to page 2", Exact = true }).ClickAsync();
        await Page.GetByText("8 entries. Showing 6 to 8.", new() { Exact = true }).WaitForAsync();
        (await dataTable.Locator(".vibe-pagination").GetAttributeAsync("aria-label")).ShouldBe("Product inventory pagination");
    }

    [Fact]
    public async Task SidebarResizesAndCollapsesWithoutHiddenFocusableContent()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/sidebar");

        var sidebar = Page.Locator(".vibe-sidebar").First;
        var handle = sidebar.Locator(".sidebar-resize-handle");
        var sidebarBox = await sidebar.BoundingBoxAsync();
        var handleBox = await handle.BoundingBoxAsync();
        sidebarBox.ShouldNotBeNull();
        handleBox.ShouldNotBeNull();

        await Page.Mouse.MoveAsync(handleBox.X + 2, handleBox.Y + handleBox.Height / 2);
        await Page.Mouse.DownAsync();
        await Page.Mouse.MoveAsync(sidebarBox.X + 270, handleBox.Y + handleBox.Height / 2, new() { Steps = 8 });
        await Page.Mouse.UpAsync();
        await Page.WaitForTimeoutAsync(220);

        (await handle.GetAttributeAsync("aria-valuenow")).ShouldBe("270");
        (await sidebar.EvaluateAsync<double>("element => element.getBoundingClientRect().width")).ShouldBe(270);
        (await handle.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();
        (await Page.EvaluateAsync<string>("() => document.body.style.cursor")).ShouldBe(string.Empty);

        await handle.PressAsync("ArrowRight");
        (await handle.GetAttributeAsync("aria-valuenow")).ShouldBe("280");
        await Page.GetByText("Sidebar: 280px", new() { Exact = true }).WaitForAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Collapse sidebar", Exact = true }).ClickAsync();
        await Page.WaitForTimeoutAsync(220);
        (await sidebar.EvaluateAsync<double>("element => element.getBoundingClientRect().width")).ShouldBe(52);
        (await sidebar.Locator(".sidebar-resize-handle").CountAsync()).ShouldBe(0);

        var content = sidebar.Locator(".sidebar-content");
        (await content.GetAttributeAsync("aria-hidden")).ShouldBe("true");
        (await content.GetAttributeAsync("inert")).ShouldNotBeNull();
        (await sidebar.GetByRole(AriaRole.Link, new() { Name = "Overview", Exact = true }).IsVisibleAsync()).ShouldBeFalse();

        var expand = Page.GetByRole(AriaRole.Button, new() { Name = "Expand sidebar", Exact = true });
        (await expand.EvaluateAsync<bool>("element => document.activeElement === element")).ShouldBeTrue();
    }

    [Fact]
    public async Task DocsCatalogUsesBaseResetAndScrollableMobileApiTables()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/sidebar");

        var breadcrumb = Page.GetByRole(AriaRole.Link, new() { Name = "Docs", Exact = true }).Last;
        (await breadcrumb.EvaluateAsync<string>("element => getComputedStyle(element).textDecorationLine")).ShouldBe("none");

        var table = Page.Locator("main .vibe-overflow-x-auto > table.vibe-w-full").First;
        var tableWidth = await table.EvaluateAsync<double>("element => element.getBoundingClientRect().width");
        var scrollerWidth = await table.Locator("xpath=..").EvaluateAsync<double>("element => element.clientWidth");
        tableWidth.ShouldBeGreaterThan(scrollerWidth);
        tableWidth.ShouldBeGreaterThanOrEqualTo(672);

        var firstRowHeight = await table.Locator("tbody tr").First.EvaluateAsync<double>("element => element.getBoundingClientRect().height");
        firstRowHeight.ShouldBeLessThanOrEqualTo(72);
    }

    [Fact]
    public async Task ConfettiRendersCanvasPixelsAndCleansUpAfterCompletion()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await NavigateAndWaitForBlazorAsync("/components/confetti");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Celebrate", Exact = true }).ClickAsync();
        var canvas = Page.Locator(".vibe-confetti-canvas");
        await canvas.WaitForAsync();
        await Page.WaitForTimeoutAsync(180);

        var nonTransparentPixels = await canvas.EvaluateAsync<int>(
            """
            element => {
                const context = element.getContext('2d');
                const pixels = context.getImageData(0, 0, element.width, element.height).data;
                let count = 0;
                for (let index = 3; index < pixels.length; index += 16) {
                    if (pixels[index] > 0) count++;
                }
                return count;
            }
            """);

        nonTransparentPixels.ShouldBeGreaterThan(0);
        (await canvas.EvaluateAsync<string>("element => getComputedStyle(element).pointerEvents")).ShouldBe("none");
        (await Page.Locator(".vibe-confetti").GetAttributeAsync("data-state")).ShouldBe("active");
        (await Page.Locator(".confetti-particle").CountAsync()).ShouldBe(0);

        await canvas.WaitForAsync(new() { State = WaitForSelectorState.Detached, Timeout = 5_000 });
        (await Page.Locator(".vibe-confetti").GetAttributeAsync("data-state")).ShouldBe("idle");
        await Page.GetByText("Celebration complete.", new() { Exact = true }).WaitForAsync();
    }

    private sealed class TableMetrics
    {
        public double ClientWidth { get; set; }
        public double ScrollWidth { get; set; }
        public string CellWhiteSpace { get; set; } = string.Empty;
    }

}
