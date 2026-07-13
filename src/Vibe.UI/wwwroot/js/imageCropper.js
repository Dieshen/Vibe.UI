const MIN_SIZE = 5;

const clamp = (value, minimum, maximum) => Math.min(Math.max(value, minimum), maximum);

export function createCropHandler(canvas, cropBox, dotNetRef, initialAspectRatio = 0) {
    let aspectRatio = Number.isFinite(initialAspectRatio) ? Math.max(0, initialAspectRatio) : 0;
    let dragState = null;
    let animationFrame = 0;
    let pendingArea = null;

    const publishArea = () => {
        animationFrame = 0;
        if (!pendingArea) return;

        const area = pendingArea;
        pendingArea = null;
        dotNetRef.invokeMethodAsync('HandlePointerCrop', area.x, area.y, area.width, area.height);
    };

    const normalizeArea = area => {
        area.width = clamp(area.width, MIN_SIZE, 100);
        area.height = clamp(area.height, MIN_SIZE, 100);
        area.x = clamp(area.x, 0, 100 - area.width);
        area.y = clamp(area.y, 0, 100 - area.height);
        return area;
    };

    const applyAspectRatio = (area, direction, start) => {
        if (aspectRatio <= 0 || direction === 'move') return area;

        const verticalOnly = !direction.includes('e') && !direction.includes('w');
        if (verticalOnly) {
            area.width = area.height * aspectRatio;
        } else {
            area.height = area.width / aspectRatio;
        }

        if (direction.includes('w')) area.x = start.x + start.width - area.width;
        if (direction.includes('n')) area.y = start.y + start.height - area.height;

        if (area.x + area.width > 100) {
            area.width = 100 - area.x;
            area.height = area.width / aspectRatio;
        }
        if (area.y + area.height > 100) {
            area.height = 100 - area.y;
            area.width = area.height * aspectRatio;
        }

        return area;
    };

    const handlePointerMove = event => {
        if (!dragState || event.pointerId !== dragState.pointerId) return;

        event.preventDefault();
        const bounds = dragState.canvasBounds;
        const deltaX = (event.clientX - dragState.clientX) / bounds.width * 100;
        const deltaY = (event.clientY - dragState.clientY) / bounds.height * 100;
        const start = dragState.area;
        const direction = dragState.direction;
        let area = { ...start };

        if (direction === 'move') {
            area.x = start.x + deltaX;
            area.y = start.y + deltaY;
        } else {
            if (direction.includes('e')) area.width = start.width + deltaX;
            if (direction.includes('s')) area.height = start.height + deltaY;
            if (direction.includes('w')) {
                area.x = start.x + deltaX;
                area.width = start.width - deltaX;
            }
            if (direction.includes('n')) {
                area.y = start.y + deltaY;
                area.height = start.height - deltaY;
            }
            area = applyAspectRatio(area, direction, start);
        }

        pendingArea = normalizeArea(area);
        if (!animationFrame) animationFrame = requestAnimationFrame(publishArea);
    };

    const stopDragging = event => {
        if (!dragState || (event.pointerId !== undefined && event.pointerId !== dragState.pointerId)) return;

        if (animationFrame) {
            cancelAnimationFrame(animationFrame);
            publishArea();
        }

        document.removeEventListener('pointermove', handlePointerMove);
        document.removeEventListener('pointerup', stopDragging);
        document.removeEventListener('pointercancel', stopDragging);
        cropBox.classList.remove('crop-box-dragging');
        document.body.style.cursor = '';
        document.body.style.userSelect = '';
        dragState = null;
    };

    const handlePointerDown = event => {
        if (event.button !== 0 || dragState) return;

        const canvasBounds = canvas.getBoundingClientRect();
        const boxBounds = cropBox.getBoundingClientRect();
        if (canvasBounds.width <= 0 || canvasBounds.height <= 0) return;

        const handle = event.target.closest('[data-crop-handle]');
        const direction = handle?.dataset.cropHandle ?? 'move';
        event.preventDefault();
        cropBox.focus({ preventScroll: true });
        dragState = {
            pointerId: event.pointerId,
            direction,
            clientX: event.clientX,
            clientY: event.clientY,
            canvasBounds,
            area: {
                x: (boxBounds.left - canvasBounds.left) / canvasBounds.width * 100,
                y: (boxBounds.top - canvasBounds.top) / canvasBounds.height * 100,
                width: boxBounds.width / canvasBounds.width * 100,
                height: boxBounds.height / canvasBounds.height * 100
            }
        };

        cropBox.classList.add('crop-box-dragging');
        document.body.style.cursor = getComputedStyle(handle ?? cropBox).cursor;
        document.body.style.userSelect = 'none';
        document.addEventListener('pointermove', handlePointerMove, { passive: false });
        document.addEventListener('pointerup', stopDragging);
        document.addEventListener('pointercancel', stopDragging);
    };

    cropBox.addEventListener('pointerdown', handlePointerDown);

    return {
        setAspectRatio(value) {
            aspectRatio = Number.isFinite(value) ? Math.max(0, value) : 0;
        },
        dispose() {
            if (animationFrame) cancelAnimationFrame(animationFrame);
            cropBox.removeEventListener('pointerdown', handlePointerDown);
            document.removeEventListener('pointermove', handlePointerMove);
            document.removeEventListener('pointerup', stopDragging);
            document.removeEventListener('pointercancel', stopDragging);
            cropBox.classList.remove('crop-box-dragging');
            document.body.style.cursor = '';
            document.body.style.userSelect = '';
            dragState = null;
        }
    };
}

export function renderCrop(canvasElement, cropBox, imageElement, zoom, rotation, flipHorizontal, flipVertical) {
    if (!canvasElement || !cropBox || !imageElement || !imageElement.complete || imageElement.naturalWidth <= 0) {
        return null;
    }

    const canvasWidth = canvasElement.clientWidth;
    const canvasHeight = canvasElement.clientHeight;
    const imageWidth = imageElement.offsetWidth;
    const imageHeight = imageElement.offsetHeight;
    if (canvasWidth <= 0 || canvasHeight <= 0 || imageWidth <= 0 || imageHeight <= 0) {
        return null;
    }

    const resolutionScale = Math.max(
        1,
        Math.min(imageElement.naturalWidth / imageWidth, imageElement.naturalHeight / imageHeight));
    const scene = document.createElement('canvas');
    scene.width = Math.max(1, Math.round(canvasWidth * resolutionScale));
    scene.height = Math.max(1, Math.round(canvasHeight * resolutionScale));
    const context = scene.getContext('2d');
    if (!context) return null;

    const centerX = (imageElement.offsetLeft + imageWidth / 2) * resolutionScale;
    const centerY = (imageElement.offsetTop + imageHeight / 2) * resolutionScale;
    context.translate(centerX, centerY);
    context.rotate((Number(rotation) || 0) * Math.PI / 180);
    context.scale(
        (Number(zoom) || 1) * (flipHorizontal ? -1 : 1),
        (Number(zoom) || 1) * (flipVertical ? -1 : 1));
    context.drawImage(
        imageElement,
        -imageWidth * resolutionScale / 2,
        -imageHeight * resolutionScale / 2,
        imageWidth * resolutionScale,
        imageHeight * resolutionScale);

    const cropX = clamp(cropBox.offsetLeft * resolutionScale, 0, scene.width - 1);
    const cropY = clamp(cropBox.offsetTop * resolutionScale, 0, scene.height - 1);
    const cropWidth = clamp(cropBox.offsetWidth * resolutionScale, 1, scene.width - cropX);
    const cropHeight = clamp(cropBox.offsetHeight * resolutionScale, 1, scene.height - cropY);
    const output = document.createElement('canvas');
    output.width = Math.max(1, Math.round(cropWidth));
    output.height = Math.max(1, Math.round(cropHeight));
    const outputContext = output.getContext('2d');
    if (!outputContext) return null;

    outputContext.drawImage(
        scene,
        cropX,
        cropY,
        cropWidth,
        cropHeight,
        0,
        0,
        output.width,
        output.height);

    try {
        return output.toDataURL('image/png');
    } catch {
        return null;
    }
}
