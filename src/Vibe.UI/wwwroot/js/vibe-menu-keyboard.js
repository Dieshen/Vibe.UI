const menuItemSelector = [
    '[role="menuitem"]',
    '[role="menuitemcheckbox"]',
    '[role="menuitemradio"]',
    '.dropdown-item',
    'button',
    'a[href]'
].join(',');

const focusableSelector = [
    'a[href]',
    'button',
    'input',
    'select',
    'textarea',
    '[tabindex]'
].join(',');

const initializedTriggers = new WeakSet();
const initializedContainers = new WeakSet();
const typeaheadState = new WeakMap();

const editableSelector = [
    'input',
    'select',
    'textarea',
    '[contenteditable]:not([contenteditable="false"])'
].join(',');

export function initializeTrigger(trigger, preventActivationKeys, includeHorizontalKeys) {
    if (!trigger || initializedTriggers.has(trigger)) {
        return;
    }

    trigger.addEventListener('keydown', event => {
        const verticalKey = event.key === 'ArrowDown' || event.key === 'ArrowUp';
        const horizontalKey = includeHorizontalKeys &&
            (event.key === 'ArrowLeft' || event.key === 'ArrowRight' || event.key === 'Home' || event.key === 'End');
        const activationKey = preventActivationKeys &&
            (event.key === 'Enter' || event.key === ' ' || event.key === 'Spacebar');

        if (verticalKey || horizontalKey || activationKey) {
            event.preventDefault();
        }
    });

    initializedTriggers.add(trigger);
}

export function initializeContainer(container, itemKind) {
    if (!container) {
        return;
    }

    if (!initializedContainers.has(container)) {
        container.addEventListener('keydown', event => {
            if (isEditableElement(event.target)) {
                return;
            }

            if (event.key === 'ArrowDown' || event.key === 'ArrowUp' || event.key === 'Home' || event.key === 'End') {
                event.preventDefault();
            }
        });

        initializedContainers.add(container);
    }

    const items = getEnabledItems(container, itemKind);
    if (items.length > 0 && !items.includes(document.activeElement)) {
        setRovingTabIndex(items, 0);
    }
}

export function isActiveElementEditable(container) {
    const activeElement = document.activeElement;
    return Boolean(container && activeElement && container.contains(activeElement) && isEditableElement(activeElement));
}

export function focusFirstItem(container, itemKind) {
    return focusBoundaryItem(container, itemKind, false);
}

export function focusLastItem(container, itemKind) {
    return focusBoundaryItem(container, itemKind, true);
}

export function moveFocus(container, itemKind, delta) {
    const items = getEnabledItems(container, itemKind);
    if (items.length === 0) {
        focusContainer(container);
        return false;
    }

    const currentIndex = items.indexOf(document.activeElement);
    const startIndex = currentIndex >= 0 ? currentIndex : (delta > 0 ? -1 : 0);
    const nextIndex = (startIndex + delta + items.length) % items.length;
    focusItem(items, nextIndex);
    return true;
}

export function focusByTypeahead(container, itemKind, key) {
    if (!container || typeof key !== 'string' || key.length !== 1 || key.trim().length === 0) {
        return false;
    }

    const items = getEnabledItems(container, itemKind);
    if (items.length === 0) {
        return false;
    }

    const normalizedKey = normalizeText(key);
    const now = performance.now();
    const previous = typeaheadState.get(container) ?? { buffer: '', timestamp: 0 };
    const buffer = now - previous.timestamp > 700
        ? normalizedKey
        : previous.buffer + normalizedKey;
    const isRepeatedCharacter = buffer.length > 1 && [...buffer].every(character => character === normalizedKey);
    const searchText = isRepeatedCharacter ? normalizedKey : buffer;

    typeaheadState.set(container, { buffer, timestamp: now });

    const currentIndex = items.indexOf(document.activeElement);
    const includeCurrentItem = searchText.length > 1 && !isRepeatedCharacter;
    const firstOffset = includeCurrentItem ? 0 : 1;

    for (let offset = firstOffset; offset < items.length + firstOffset; offset++) {
        const index = (Math.max(currentIndex, -1) + offset + items.length) % items.length;
        if (getItemText(items[index]).startsWith(searchText)) {
            focusItem(items, index);
            return true;
        }
    }

    return false;
}

function focusBoundaryItem(container, itemKind, useLastItem) {
    const items = getEnabledItems(container, itemKind);
    if (items.length === 0) {
        focusContainer(container);
        return false;
    }

    focusItem(items, useLastItem ? items.length - 1 : 0);
    return true;
}

function focusItem(items, index) {
    setRovingTabIndex(items, index);
    items[index].focus({ preventScroll: true });
}

function focusContainer(container) {
    if (container && typeof container.focus === 'function') {
        container.focus({ preventScroll: true });
    }
}

function setRovingTabIndex(items, activeIndex) {
    items.forEach((item, index) => {
        item.tabIndex = index === activeIndex ? 0 : -1;
    });
}

function getEnabledItems(container, itemKind) {
    if (!container) {
        return [];
    }

    const selector = itemKind === 'focusable' ? focusableSelector : menuItemSelector;
    const candidates = Array.from(container.querySelectorAll(selector))
        .filter(element => isVisible(element) && !isDisabled(element));

    return candidates.filter(element =>
        (itemKind !== 'focusable' || !isEditableElement(element)) &&
        !candidates.some(candidate => candidate !== element && candidate.contains(element)));
}

function isEditableElement(element) {
    return element instanceof Element && element.matches(editableSelector);
}

function isVisible(element) {
    return !element.hidden &&
        element.getAttribute('aria-hidden') !== 'true' &&
        !element.closest('[hidden], [aria-hidden="true"], [inert]') &&
        element.getClientRects().length > 0;
}

function isDisabled(element) {
    return element.matches(':disabled') ||
        element.getAttribute('aria-disabled') === 'true' ||
        element.closest('[aria-disabled="true"]') !== null;
}

function getItemText(element) {
    return normalizeText(element.getAttribute('aria-label') || element.textContent || '');
}

function normalizeText(value) {
    return value
        .normalize('NFD')
        .replace(/[\u0300-\u036f]/g, '')
        .trim()
        .replace(/\s+/g, ' ')
        .toLocaleLowerCase();
}
