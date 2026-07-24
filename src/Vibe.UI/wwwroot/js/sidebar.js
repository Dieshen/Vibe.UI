const connections = new WeakMap();

function readStoredWidth(key) {
    if (!key) {
        return null;
    }

    try {
        const value = Number.parseFloat(localStorage.getItem(key) ?? "");
        return Number.isFinite(value) ? value : null;
    } catch {
        return null;
    }
}

export function save(key, width) {
    if (!key) {
        return;
    }

    try {
        localStorage.setItem(key, String(width));
    } catch {
    }
}

export function connect(root, handle, dotNetRef, isLeft, persistenceKey) {
    disconnect(root);

    let pointerId = null;
    let animationFrame = 0;
    let pendingWidth = null;

    const publishWidth = () => {
        animationFrame = 0;
        if (pendingWidth === null) {
            return;
        }

        const width = pendingWidth;
        pendingWidth = null;
        save(persistenceKey, width);
        dotNetRef.invokeMethodAsync("HandleResize", width);
    };

    const handlePointerMove = event => {
        if (pointerId !== event.pointerId) {
            return;
        }

        const bounds = root.getBoundingClientRect();
        pendingWidth = isLeft ? event.clientX - bounds.left : bounds.right - event.clientX;
        if (!animationFrame) {
            animationFrame = requestAnimationFrame(publishWidth);
        }
    };

    const stopDragging = event => {
        if (pointerId === null || (event.pointerId !== undefined && event.pointerId !== pointerId)) {
            return;
        }

        if (animationFrame) {
            cancelAnimationFrame(animationFrame);
            publishWidth();
        }

        if (handle.hasPointerCapture?.(pointerId)) {
            handle.releasePointerCapture(pointerId);
        }

        pointerId = null;
        root.classList.remove("resizing");
        document.body.style.cursor = "";
        document.body.style.userSelect = "";
    };

    const handlePointerDown = event => {
        if (event.button !== 0 || pointerId !== null) {
            return;
        }

        event.preventDefault();
        handle.focus({ preventScroll: true });
        pointerId = event.pointerId;
        handle.setPointerCapture?.(pointerId);
        root.classList.add("resizing");
        document.body.style.cursor = "col-resize";
        document.body.style.userSelect = "none";
    };

    handle.addEventListener("pointerdown", handlePointerDown);
    handle.addEventListener("pointermove", handlePointerMove);
    handle.addEventListener("pointerup", stopDragging);
    handle.addEventListener("pointercancel", stopDragging);

    connections.set(root, {
        handle,
        handlePointerDown,
        handlePointerMove,
        stopDragging,
        dispose: () => {
            if (animationFrame) {
                cancelAnimationFrame(animationFrame);
            }
            root.classList.remove("resizing");
            document.body.style.cursor = "";
            document.body.style.userSelect = "";
        }
    });

    return readStoredWidth(persistenceKey);
}

export function disconnect(root) {
    const connection = connections.get(root);
    if (!connection) {
        return;
    }

    connection.handle.removeEventListener("pointerdown", connection.handlePointerDown);
    connection.handle.removeEventListener("pointermove", connection.handlePointerMove);
    connection.handle.removeEventListener("pointerup", connection.stopDragging);
    connection.handle.removeEventListener("pointercancel", connection.stopDragging);
    connection.dispose();
    connections.delete(root);
}
