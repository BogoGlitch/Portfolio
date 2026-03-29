'use client';

import { useEffect, useRef, useState } from 'react';
import { useTypewriter } from '@/hooks/useTypewriter';
import { useDocumentTheme } from '@/hooks/useDocumentTheme';
import styles from './HeroTypewriter.module.css';

const STRINGS = [
  'Backend-first engineering.',
  'Enterprise architecture.',
  'API design & delivery.',
  'Full-stack, end to end.',
];

const GLITCH_CHARS = '!@#$[]{}|<>/\\~`_=+';
const LONGEST = STRINGS.reduce((a, b) => (a.length >= b.length ? a : b));

// Cycles through string indices: pause → transition → advance → repeat
function useCycler(length: number, pauseMs: number, animMs: number) {
  const [curr, setCurr] = useState(0);
  const [transitioning, setTransitioning] = useState(false);

  useEffect(() => {
    const t = setTimeout(() => {
      setTransitioning(true);
      const t2 = setTimeout(() => {
        setCurr(i => (i + 1) % length);
        setTransitioning(false);
      }, animMs);
      return () => clearTimeout(t2);
    }, pauseMs);
    return () => clearTimeout(t);
  }, [curr, length, pauseMs, animMs]);

  return { curr, next: (curr + 1) % length, transitioning };
}

export default function HeroTypewriter() {
  const theme = useDocumentTheme();
  const [reducedMotion, setReducedMotion] = useState(false);

  // Glitch scramble state
  const [glitchOverlay, setGlitchOverlay] = useState<string | null>(null);
  const displayedRef = useRef('');
  const glitchTimer = useRef<ReturnType<typeof setTimeout> | undefined>(undefined);

  useEffect(() => {
    const mq = window.matchMedia('(prefers-reduced-motion: reduce)');
    setReducedMotion(mq.matches);
  }, []);

  // Always run typewriter; only rendered when theme === 'glitch'
  const displayed = useTypewriter(STRINGS, 45, 1800);
  useEffect(() => { displayedRef.current = displayed; }, [displayed]);

  // Glitch scramble: one char briefly corrupts to a symbol then snaps back
  useEffect(() => {
    if (theme !== 'glitch' || reducedMotion) {
      setGlitchOverlay(null);
      return;
    }
    let cancelled = false;
    function scheduleGlitch() {
      glitchTimer.current = setTimeout(() => {
        if (cancelled) return;
        const cur = displayedRef.current;
        if (cur.length > 0) {
          const idx = Math.floor(Math.random() * cur.length);
          const gc = GLITCH_CHARS[Math.floor(Math.random() * GLITCH_CHARS.length)];
          setGlitchOverlay(cur.slice(0, idx) + gc + cur.slice(idx + 1));
          setTimeout(() => {
            if (!cancelled) { setGlitchOverlay(null); scheduleGlitch(); }
          }, 80);
        } else {
          scheduleGlitch();
        }
      }, 700 + Math.random() * 1300);
    }
    scheduleGlitch();
    return () => {
      cancelled = true;
      clearTimeout(glitchTimer.current);
      setGlitchOverlay(null);
    };
  }, [theme, reducedMotion]);

  // Cycler for Ember + Cosmos (runs always, only rendered for those themes)
  const { curr, next, transitioning } = useCycler(STRINGS.length, 2800, 440);

  if (reducedMotion) {
    return <span className={styles.typewriter}>{STRINGS[0]}</span>;
  }

  // ── Glitch: typewriter + scramble ──
  if (theme === 'glitch') {
    return (
      <span className={styles.typewriter}>
        {glitchOverlay ?? displayed}
        <span className={styles.cursorGlitch} aria-hidden="true" />
      </span>
    );
  }

  // ── Ember: roll up ──
  if (theme === 'ember') {
    return (
      <span className={styles.cycleWrapper}>
        <span className={styles.placeholder} aria-hidden="true">{LONGEST}</span>
        <span className={`${styles.slot} ${transitioning ? styles.emberExit : ''}`}>
          {STRINGS[curr]}
        </span>
        {transitioning && (
          <span className={`${styles.slot} ${styles.emberEnter}`}>
            {STRINGS[next]}
          </span>
        )}
      </span>
    );
  }

  // ── Prism: refract — text disperses like light through a prism ──
  if (theme === 'prism') {
    return (
      <span className={styles.cycleWrapper}>
        <span className={styles.placeholder} aria-hidden="true">{LONGEST}</span>
        <span className={`${styles.slot} ${transitioning ? styles.prismExit : ''}`}>
          {STRINGS[curr]}
        </span>
        {transitioning && (
          <span className={`${styles.slot} ${styles.prismEnter}`}>
            {STRINGS[next]}
          </span>
        )}
      </span>
    );
  }

  // ── Cosmos: slide in from left, out to right ──
  return (
    <span className={styles.cycleWrapper}>
      <span className={styles.placeholder} aria-hidden="true">{LONGEST}</span>
      <span className={`${styles.slot} ${transitioning ? styles.cosmosExit : ''}`}>
        {STRINGS[curr]}
      </span>
      {transitioning && (
        <span className={`${styles.slot} ${styles.cosmosEnter}`}>
          {STRINGS[next]}
        </span>
      )}
    </span>
  );
}
