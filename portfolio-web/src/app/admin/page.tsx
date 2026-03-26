'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { TbDatabase, TbCode } from 'react-icons/tb';
import { useAuth } from '@/hooks/useAuth';
import styles from './admin.module.css';

export default function AdminPage() {
  const { isLoggedIn, isLoading } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!isLoading && !isLoggedIn) router.push('/');
  }, [isLoading, isLoggedIn, router]);

  if (isLoading || !isLoggedIn) return null;

  return (
    <main className={styles.page}>
      <h1 className={styles.heading}>Admin</h1>
      <p className={styles.sub}>Manage site content</p>

      <div className={styles.grid}>
        <Link href="/admin/technologies" className={styles.card}>
          <TbCode size={28} className={styles.cardIcon} />
          <span className={styles.cardLabel}>Technologies</span>
        </Link>
        <Link href="/admin/projects" className={styles.card}>
          <TbDatabase size={28} className={styles.cardIcon} />
          <span className={styles.cardLabel}>Projects</span>
        </Link>
      </div>
    </main>
  );
}
