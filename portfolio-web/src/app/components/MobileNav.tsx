'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { TbFolder, TbCpu, TbBrain, TbX } from 'react-icons/tb';
import { useTheme } from '@/hooks/useTheme';
import styles from './MobileNav.module.css';

const NAV_LINKS = [
  { href: '/projects',     label: 'Projects',     Icon: TbFolder },
  { href: '/technologies', label: 'Technologies', Icon: TbCpu },
  { href: null,            label: 'Approach',     Icon: TbBrain, disabled: true },
];

export default function MobileNav() {
  const [open, setOpen] = useState(false);
  const { theme } = useTheme();

  const close = () => setOpen(false);

  // Close if the viewport grows past the mobile breakpoint
  useEffect(() => {
    const onResize = () => { if (window.innerWidth >= 640) close(); };
    window.addEventListener('resize', onResize);
    return () => window.removeEventListener('resize', onResize);
  }, []);

  // Prevent body scroll when drawer is open
  useEffect(() => {
    document.body.style.overflow = open ? 'hidden' : '';
    return () => { document.body.style.overflow = ''; };
  }, [open]);

  return (
    <>
      {/* ── Hamburger button ── */}
      <button
        className={styles.toggle}
        data-open={open || undefined}
        data-theme={theme}
        onClick={() => setOpen(prev => !prev)}
        aria-label={open ? 'Close menu' : 'Open menu'}
        aria-expanded={open}
      >
        <span className={styles.bar} />
        <span className={styles.bar} />
        <span className={styles.bar} />
      </button>

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
        <div className={styles.drawerHeader}>
          <span className={styles.drawerBrand}>Menu</span>
          <button className={styles.closeBtn} onClick={close} aria-label="Close menu">
            <TbX size={20} />
          </button>
        </div>

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
}
