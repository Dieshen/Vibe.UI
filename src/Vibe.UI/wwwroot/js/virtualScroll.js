const registrations = new WeakMap();

export function connect(element, dotNetReference) {
    disconnect(element);

    let animationFrame = 0;
    let connected = true;

    const onScroll = () => {
        if (animationFrame !== 0) {
            return;
        }

        animationFrame = requestAnimationFrame(() => {
            animationFrame = 0;
            if (!connected) {
                return;
            }

            dotNetReference
                .invokeMethodAsync("HandleScrollFromJs", Math.round(element.scrollTop))
                .catch(() => {
                    connected = false;
                });
        });
    };

    element.addEventListener("scroll", onScroll, { passive: true });
    registrations.set(element, {
        dispose() {
            connected = false;
            element.removeEventListener("scroll", onScroll);
            if (animationFrame !== 0) {
                cancelAnimationFrame(animationFrame);
            }
        }
    });
}

export function disconnect(element) {
    registrations.get(element)?.dispose();
    registrations.delete(element);
}

export function scrollTo(element, scrollTop) {
    element.scrollTop = Math.max(0, Number(scrollTop) || 0);
}
