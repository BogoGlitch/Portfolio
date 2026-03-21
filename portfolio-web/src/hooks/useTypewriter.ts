'use client';

import { useEffect, useState } from 'react';

export function useTypewriter(strings: string[], speed = 60, pauseMs = 2200) {
  const [displayed, setDisplayed] = useState('');
  const [stringIndex, setStringIndex] = useState(0);
  const [charIndex, setCharIndex] = useState(0);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    const current = strings[stringIndex];

    if (!deleting && charIndex === current.length) {
      const t = setTimeout(() => setDeleting(true), pauseMs);
      return () => clearTimeout(t);
    }

    if (deleting && charIndex === 0) {
      setDeleting(false);
      setStringIndex((i) => (i + 1) % strings.length);
      return;
    }

    const t = setTimeout(() => {
      const next = charIndex + (deleting ? -1 : 1);
      setCharIndex(next);
      setDisplayed(current.slice(0, next));
    }, deleting ? speed / 2 : speed);

    return () => clearTimeout(t);
  }, [charIndex, deleting, stringIndex, strings, speed, pauseMs]);

  return displayed;
}
