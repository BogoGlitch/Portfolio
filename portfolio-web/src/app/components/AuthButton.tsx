'use client';

import { useState } from 'react';
import Link from 'next/link';
import { TbLogin2, TbLogout, TbLayoutDashboard } from 'react-icons/tb';
import { useAuth } from '@/hooks/useAuth';
import LoginModal from './LoginModal';
import styles from './AuthButton.module.css';

export default function AuthButton() {
  const { isLoggedIn, isLoading, login, logout } = useAuth();
  const [modalOpen, setModalOpen] = useState(false);

  if (isLoading) return null;

  if (!isLoggedIn) {
    return (
      <>
        <button
          className={styles.iconBtn}
          onClick={() => setModalOpen(true)}
          aria-label="Sign in"
          title="Sign in"
        >
          <TbLogin2 size={17} />
        </button>
        {modalOpen && (
          <LoginModal onLogin={login} onClose={() => setModalOpen(false)} />
        )}
      </>
    );
  }

  return (
    <>
      <Link href="/admin" className={styles.iconBtn} aria-label="Admin dashboard" title="Admin dashboard">
        <TbLayoutDashboard size={17} />
      </Link>
      <button
        className={styles.iconBtn}
        onClick={logout}
        aria-label="Sign out"
        title="Sign out"
      >
        <TbLogout size={17} />
      </button>
    </>
  );
}
