'use client';

import { useEffect, useState } from 'react';
import { THEMES, type Theme } from './useTheme';

function readTheme(): Theme {
  const attr = document.documentElement.getAttribute('data-theme') as Theme;
  return THEMES.includes(attr) ? attr : 'cosmos';
}

/**
 * Read-only hook — returns the current theme, updates on data-theme changes.
 * Initialises to 'cosmos' so server and client render the same initial HTML
 * (no hydration mismatch). After mount, syncs to the real DOM value set by
 * the FOUC inline script, then watches for changes via MutationObserver.
 */
export function useDocumentTheme(): Theme {
  const [theme, setTheme] = useState<Theme>('cosmos');

  useEffect(() => {
    // Sync to actual value immediately after mount
    setTheme(readTheme());

    const observer = new MutationObserver(() => setTheme(readTheme()));
    observer.observe(document.documentElement, {
      attributes: true,
      attributeFilter: ['data-theme'],
    });
    return () => observer.disconnect();
  }, []);

  return theme;
}
