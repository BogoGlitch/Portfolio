'use client';

import { useEffect, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import Link from 'next/link';
import { TbFolder, TbCpu, TbBrain } from 'react-icons/tb';
import { useDocumentTheme } from '@/hooks/useDocumentTheme';
import styles from './MobileNav.module.css';

const NAV_LINKS = [
  { href: '/projects',     label: 'Projects',     Icon: TbFolder },
  { href: '/technologies', label: 'Technologies', Icon: TbCpu },
  { href: null,            label: 'Approach',     Icon: TbBrain, disabled: true },
];

const ANIM_DURATION_OPEN = 460;
const CLOSE_DURATION: Record<string, number> = {
  glitch: 280,
  ember:  420,
  cosmos: 520,
};

export default function MobileNav() {
  const [open, setOpen] = useState(false);
  const [closing, setClosing] = useState(false);
  const [mounted, setMounted] = useState(false);
  const closeTimer = useRef<ReturnType<typeof setTimeout> | null>(null);
  const theme = useDocumentTheme();

  // Immediate close — used by links and resize (no reverse animation needed)
  const close = () => {
    if (closeTimer.current) clearTimeout(closeTimer.current);
    setClosing(false);
    setOpen(false);
  };

  // Animated close — drawer closes immediately, hamburger animates in parallel
  const animatedClose = () => {
    setOpen(false);
    setClosing(true);
    closeTimer.current = setTimeout(() => {
      setClosing(false);
    }, CLOSE_DURATION[theme] ?? 420);
  };

  const handleToggle = () => {
    if (!open) {
      setOpen(true);
    } else if (!closing) {
      animatedClose();
    }
  };

  // Wait for client mount before portaling (SSR safety)
  useEffect(() => { setMounted(true); }, []);

  // Close if the viewport grows past the mobile breakpoint
  useEffect(() => {
    const onResize = () => { if (window.innerWidth >= 640) close(); };
    window.addEventListener('resize', onResize);
    return () => window.removeEventListener('resize', onResize);
  }, []);

  // Cleanup timer on unmount
  useEffect(() => () => { if (closeTimer.current) clearTimeout(closeTimer.current); }, []);

  // Prevent body scroll when drawer is open (works on iOS Safari too)
  useEffect(() => {
    if (open) {
      const y = window.scrollY;
      document.body.style.position = 'fixed';
      document.body.style.top = `-${y}px`;
      document.body.style.width = '100%';
    } else {
      const y = Math.abs(parseInt(document.body.style.top || '0', 10));
      document.body.style.position = '';
      document.body.style.top = '';
      document.body.style.width = '';
      if (y) window.scrollTo(0, y);
    }
    return () => {
      const y = Math.abs(parseInt(document.body.style.top || '0', 10));
      document.body.style.position = '';
      document.body.style.top = '';
      document.body.style.width = '';
    };
  }, [open]);

  const overlay = (
    <>
      {/* ── Backdrop ── */}
      <div
        className={`${styles.backdrop} ${open ? styles.backdropOpen : ''}`}
        onClick={close}
        aria-hidden="true"
      />

      {/* ── Drawer ── */}
      <nav
        className={`${styles.drawer} ${open ? styles.drawerOpen : ''}`}
        data-theme={theme}
        aria-label="Mobile navigation"
        aria-hidden={!open}
      >
        <div className={styles.drawerLinks}>
          {NAV_LINKS.map(({ href, label, Icon, disabled }) =>
            disabled || !href ? (
              <span key={label} className={`${styles.link} ${styles.linkDisabled}`}>
                <Icon size={20} className={styles.linkIcon} />
                <span className={styles.linkLabel}>{label}</span>
                <span className={styles.soon}>soon</span>
              </span>
            ) : (
              <Link key={label} href={href} className={styles.link} onClick={close}>
                <Icon size={20} className={styles.linkIcon} />
                <span className={styles.linkLabel}>{label}</span>
                <span className={styles.linkArrow}>→</span>
              </Link>
            )
          )}
        </div>
      </nav>
    </>
  );

  return (
    <>
      {/* ── Hamburger button — stays inside the header ── */}
      <button
        className={styles.toggle}
        data-open={open && !closing ? true : undefined}
        data-closing={closing ? true : undefined}
        data-theme={theme}
        onClick={handleToggle}
        aria-label={open ? 'Close menu' : 'Open menu'}
        aria-expanded={open}
      >
        <span className={styles.bar} />
        <span className={styles.bar} />
        <span className={styles.bar} />
      </button>

      {/* ── Backdrop + Drawer portaled to body, outside header stacking context ── */}
      {mounted && createPortal(overlay, document.body)}
    </>
  );
}
