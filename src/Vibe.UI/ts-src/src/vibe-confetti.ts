import confetti, { type Options } from "canvas-confetti";

export interface ConfettiOptions {
  particleCount: number;
  duration: number;
  colors: string[];
  origin: string;
  pattern: string;
}

interface ConfettiState {
  canvas: HTMLCanvasElement;
  instance: ReturnType<typeof confetti.create>;
}

const states = new WeakMap<HTMLElement, ConfettiState>();

function resolveOrigin(origin: string): { x: number; y: number } {
  switch (origin) {
    case "top": return { x: 0.5, y: 0 };
    case "bottom": return { x: 0.5, y: 1 };
    case "left": return { x: 0, y: 0.5 };
    case "right": return { x: 1, y: 0.5 };
    case "topleft": return { x: 0, y: 0 };
    case "topright": return { x: 1, y: 0 };
    case "bottomleft": return { x: 0, y: 1 };
    case "bottomright": return { x: 1, y: 1 };
    default: return { x: 0.5, y: 0.5 };
  }
}

function baseOptions(options: ConfettiOptions): Options {
  return {
    particleCount: Math.max(0, options.particleCount),
    colors: options.colors,
    origin: resolveOrigin(options.origin),
    ticks: Math.max(1, Math.round(Math.max(0, options.duration) / (1000 / 60))),
    disableForReducedMotion: true,
    zIndex: 1100
  };
}

function patternOptions(options: ConfettiOptions): Options[] {
  const base = baseOptions(options);
  switch (options.pattern) {
    case "fountain":
      return [{ ...base, angle: 90, spread: 55, startVelocity: 48, gravity: 1.15 }];
    case "rain":
      return [{ ...base, angle: 270, spread: 110, startVelocity: 18, gravity: 0.75, origin: { x: base.origin?.x ?? 0.5, y: 0 } }];
    case "fireworks": {
      const count = Math.max(1, Math.round((base.particleCount ?? 1) / 2));
      return [
        { ...base, particleCount: count, spread: 65, startVelocity: 52, origin: { x: 0.32, y: 0.42 } },
        { ...base, particleCount: count, spread: 65, startVelocity: 52, origin: { x: 0.68, y: 0.42 } }
      ];
    }
    default:
      return [{ ...base, spread: 80, startVelocity: 45, gravity: 1 }];
  }
}

export async function trigger(root: HTMLElement, options: ConfettiOptions): Promise<void> {
  stop(root);

  const canvas = document.createElement("canvas");
  canvas.className = "vibe-confetti-canvas";
  canvas.setAttribute("aria-hidden", "true");
  root.appendChild(canvas);

  const instance = confetti.create(canvas, { resize: true, useWorker: false });
  const state = { canvas, instance };
  states.set(root, state);

  try {
    await Promise.all(patternOptions(options).map(pattern => instance(pattern)));
  } finally {
    if (states.get(root) === state) {
      states.delete(root);
      canvas.remove();
    }
  }
}

export function stop(root: HTMLElement): void {
  const state = states.get(root);
  if (!state) {
    return;
  }

  state.instance.reset();
  state.canvas.remove();
  states.delete(root);
}
