/**
 * Vibe.UI Resizable Component JS Module
 * Handles drag operations for resizable panels
 */

export function createDragHandler(root, handle, dotNetRef, isHorizontal) {
    let pointerId = null;
    let animationFrame = 0;
    let pendingSize = null;

    const publishSize = () => {
        animationFrame = 0;
        if (pendingSize === null) return;

        const size = pendingSize;
        pendingSize = null;
        dotNetRef.invokeMethodAsync('HandleDragMove', size);
    };

    const handlePointerMove = event => {
        if (pointerId !== event.pointerId) return;

        const bounds = root.getBoundingClientRect();
        pendingSize = isHorizontal
            ? event.clientX - bounds.left
            : event.clientY - bounds.top;

        if (!animationFrame) {
            animationFrame = requestAnimationFrame(publishSize);
        }
    };

    const stopDragging = event => {
        if (pointerId === null || (event.pointerId !== undefined && event.pointerId !== pointerId)) return;

        if (animationFrame) {
            cancelAnimationFrame(animationFrame);
            publishSize();
        }

        if (handle.hasPointerCapture?.(pointerId)) {
            handle.releasePointerCapture(pointerId);
        }

        pointerId = null;
        root.classList.remove('resizing');
        document.body.style.cursor = '';
        document.body.style.userSelect = '';
        dotNetRef.invokeMethodAsync('HandleDragEnd');
    };

    const handlePointerDown = event => {
        if (event.button !== 0 || pointerId !== null) return;

        event.preventDefault();
        handle.focus({ preventScroll: true });
        pointerId = event.pointerId;
        handle.setPointerCapture?.(pointerId);
        root.classList.add('resizing');
        document.body.style.cursor = isHorizontal ? 'col-resize' : 'row-resize';
        document.body.style.userSelect = 'none';
    };

    handle.addEventListener('pointerdown', handlePointerDown);
    handle.addEventListener('pointermove', handlePointerMove);
    handle.addEventListener('pointerup', stopDragging);
    handle.addEventListener('pointercancel', stopDragging);

    return {
        dispose() {
            if (animationFrame) cancelAnimationFrame(animationFrame);
            handle.removeEventListener('pointerdown', handlePointerDown);
            handle.removeEventListener('pointermove', handlePointerMove);
            handle.removeEventListener('pointerup', stopDragging);
            handle.removeEventListener('pointercancel', stopDragging);
            root.classList.remove('resizing');
            document.body.style.cursor = '';
            document.body.style.userSelect = '';
        }
    };
}
