'use client';

import { useEffect, useState } from 'react';
import { useIntersectionObserver } from '@/hooks/useIntersectionObserver';
import { useDocumentTheme } from '@/hooks/useDocumentTheme';

type Props = {
  children: React.ReactNode;
  className?: string;
  delay?: number;
  as?: React.ElementType;
  /** Skip opacity:0 start — use for cards already in the initial viewport. */
  instant?: boolean;
};

export default function AnimatedSection({ children, className = '', delay = 0, as: Tag = 'div', instant = false }: Props) {
  const { ref, entry } = useIntersectionObserver();
  const theme = useDocumentTheme();
  const [animTheme, setAnimTheme] = useState<string | null>(null);

  useEffect(() => {
    if (!entry) return;

    if (entry.isIntersecting) {
      // Entered viewport — animate with current theme.
      // Also re-fires when theme changes while visible, replaying the animation.
      setAnimTheme(theme);
    } else if (entry.boundingClientRect.top > 0) {
      // Left via the bottom (user scrolled back up) — reset so it re-animates.
      setAnimTheme(null);
    }
    // Left via the top (user scrolled past it going down) — leave it visible.
  }, [entry, theme]);

  const visibleClass = animTheme ? `visible anim-theme-${animTheme}` : '';

  if (instant) {
    return <Tag className={className || undefined}>{children}</Tag>;
  }

  return (
    <Tag
      ref={ref}
      className={`animate-in${visibleClass ? ` ${visibleClass}` : ''}${className ? ` ${className}` : ''}`}
      style={{ animationDelay: `${delay}ms` }}
    >
      {children}
    </Tag>
  );
}
