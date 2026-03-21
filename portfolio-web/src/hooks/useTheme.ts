'use client';

import { useCallback, useEffect, useLayoutEffect, useState } from 'react';

export const THEMES = ['glitch', 'ember', 'cosmos'] as const;
export type Theme = typeof THEMES[number];

const TRANSITION_DURATION = 320;

// useLayoutEffect on client (fires before paint — no flash),
// useEffect on server (doesn't run anyway, avoids the SSR warning).
const useIsomorphicLayoutEffect =
  typeof window !== 'undefined' ? useLayoutEffect : useEffect;

export function useTheme() {
  const [theme, setTheme] = useState<Theme>('glitch');

  // Sync from localStorage before first paint so the pill never flashes 'glitch'
  useIsomorphicLayoutEffect(() => {
    const stored = localStorage.getItem('portfolio-theme') as Theme | null;
    if (stored && THEMES.includes(stored)) {
      setTheme(stored);
    }
  }, []);

  const cycleTheme = useCallback(() => {
    const next = THEMES[(THEMES.indexOf(theme) + 1) % THEMES.length];

    // Add transition class so colors animate during theme change only —
    // not during normal hover interactions (fixes tech tag flash).
    document.documentElement.classList.add('theme-transitioning');
    document.documentElement.setAttribute('data-theme', next);
    localStorage.setItem('portfolio-theme', next);
    setTheme(next);

    setTimeout(() => {
      document.documentElement.classList.remove('theme-transitioning');
    }, TRANSITION_DURATION);
  }, [theme]);

  return { theme, cycleTheme };
}
