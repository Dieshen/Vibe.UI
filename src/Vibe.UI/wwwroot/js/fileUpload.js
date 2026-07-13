const connections = new WeakMap();

function containsFiles(event) {
    return event.dataTransfer?.types?.includes("Files") === true;
}

function isDisabled(root) {
    return root.getAttribute("aria-disabled") === "true";
}

function setDragging(root, active) {
    root.classList.toggle("file-upload-dragging", active);
}

export function connect(root) {
    disconnect(root);

    let dragDepth = 0;

    const handleDragEnter = event => {
        if (isDisabled(root) || !containsFiles(event)) {
            return;
        }

        event.preventDefault();
        dragDepth += 1;
        setDragging(root, true);
    };

    const handleDragOver = event => {
        if (isDisabled(root) || !containsFiles(event)) {
            return;
        }

        event.preventDefault();
        event.dataTransfer.dropEffect = "copy";
    };

    const handleDragLeave = event => {
        if (!containsFiles(event)) {
            return;
        }

        dragDepth = Math.max(0, dragDepth - 1);
        if (dragDepth === 0) {
            setDragging(root, false);
        }
    };

    const handleDrop = event => {
        if (!containsFiles(event)) {
            return;
        }

        event.preventDefault();
        dragDepth = 0;
        setDragging(root, false);

        if (isDisabled(root) || event.dataTransfer.files.length === 0) {
            return;
        }

        const input = root.querySelector("input[type='file']:not(:disabled)");
        if (!input) {
            return;
        }

        const transfer = new DataTransfer();
        for (const file of event.dataTransfer.files) {
            transfer.items.add(file);
            if (!input.multiple) {
                break;
            }
        }

        input.files = transfer.files;
        input.dispatchEvent(new Event("change", { bubbles: true }));
    };

    root.addEventListener("dragenter", handleDragEnter);
    root.addEventListener("dragover", handleDragOver);
    root.addEventListener("dragleave", handleDragLeave);
    root.addEventListener("drop", handleDrop);

    connections.set(root, {
        handleDragEnter,
        handleDragOver,
        handleDragLeave,
        handleDrop
    });
}

export function disconnect(root) {
    const connection = connections.get(root);
    if (!connection) {
        return;
    }

    root.removeEventListener("dragenter", connection.handleDragEnter);
    root.removeEventListener("dragover", connection.handleDragOver);
    root.removeEventListener("dragleave", connection.handleDragLeave);
    root.removeEventListener("drop", connection.handleDrop);
    setDragging(root, false);
    connections.delete(root);
}
