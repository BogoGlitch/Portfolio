'use client';

import { useEffect, useRef, useState } from 'react';

export function useScrollDirection(threshold = 8) {
  const [hidden, setHidden] = useState(false);
  const lastY = useRef(0);

  useEffect(() => {
    const onScroll = () => {
      const y = window.scrollY;
      const delta = y - lastY.current;

      if (Math.abs(delta) < threshold) return;

      // Always show at the very top
      if (y <= 0) {
        setHidden(false);
      } else if (delta > 0) {
        // Scrolling down — hide
        setHidden(true);
      } else {
        // Scrolling up — show
        setHidden(false);
      }

      lastY.current = y;
    };

    window.addEventListener('scroll', onScroll, { passive: true });
    return () => window.removeEventListener('scroll', onScroll);
  }, [threshold]);

  return hidden;
}
