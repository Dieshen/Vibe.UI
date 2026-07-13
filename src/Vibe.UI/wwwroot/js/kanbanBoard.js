export function focusCard(root, cardId) {
    const cards = root.querySelectorAll("[data-kanban-card-id]");
    for (const card of cards) {
        if (card.getAttribute("data-kanban-card-id") === cardId) {
            card.focus({ preventScroll: true });
            return true;
        }
    }

    return false;
}
