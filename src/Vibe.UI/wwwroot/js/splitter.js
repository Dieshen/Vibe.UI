const connections = new WeakMap();

export function connect(root, divider, dotNetRef, isHorizontal) {
    disconnect(root);

    let pointerId = null;
    let animationFrame = 0;
    let pendingSize = null;

    const publishSize = () => {
        animationFrame = 0;
        if (pendingSize === null) {
            return;
        }

        const size = pendingSize;
        pendingSize = null;
        dotNetRef.invokeMethodAsync("HandleDragMove", size);
    };

    const handlePointerMove = event => {
        if (pointerId !== event.pointerId) {
            return;
        }

        const bounds = root.getBoundingClientRect();
        const availableSize = isHorizontal ? bounds.width : bounds.height;
        if (availableSize <= 0) {
            return;
        }

        const offset = isHorizontal
            ? event.clientX - bounds.left
            : event.clientY - bounds.top;
        pendingSize = Math.max(0, Math.min(100, (offset / availableSize) * 100));
        if (!animationFrame) {
            animationFrame = requestAnimationFrame(publishSize);
        }
    };

    const stopDragging = event => {
        if (pointerId === null || (event.pointerId !== undefined && event.pointerId !== pointerId)) {
            return;
        }

        if (animationFrame) {
            cancelAnimationFrame(animationFrame);
            publishSize();
        }

        if (divider.hasPointerCapture?.(pointerId)) {
            divider.releasePointerCapture(pointerId);
        }

        pointerId = null;
        root.classList.remove("dragging");
        document.body.style.cursor = "";
        document.body.style.userSelect = "";
    };

    const handlePointerDown = event => {
        if (event.button !== 0 || pointerId !== null) {
            return;
        }

        event.preventDefault();
        divider.focus({ preventScroll: true });
        pointerId = event.pointerId;
        divider.setPointerCapture?.(pointerId);
        root.classList.add("dragging");
        document.body.style.cursor = isHorizontal ? "col-resize" : "row-resize";
        document.body.style.userSelect = "none";
    };

    divider.addEventListener("pointerdown", handlePointerDown);
    divider.addEventListener("pointermove", handlePointerMove);
    divider.addEventListener("pointerup", stopDragging);
    divider.addEventListener("pointercancel", stopDragging);

    connections.set(root, {
        divider,
        handlePointerDown,
        handlePointerMove,
        stopDragging,
        dispose: () => {
            if (animationFrame) {
                cancelAnimationFrame(animationFrame);
            }
            pointerId = null;
            root.classList.remove("dragging");
            document.body.style.cursor = "";
            document.body.style.userSelect = "";
        }
    });
}

export function disconnect(root) {
    const connection = connections.get(root);
    if (!connection) {
        return;
    }

    connection.divider.removeEventListener("pointerdown", connection.handlePointerDown);
    connection.divider.removeEventListener("pointermove", connection.handlePointerMove);
    connection.divider.removeEventListener("pointerup", connection.stopDragging);
    connection.divider.removeEventListener("pointercancel", connection.stopDragging);
    connection.dispose();
    connections.delete(root);
}
