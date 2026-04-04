'use client';

import Link from 'next/link';
import { TbDatabase, TbCode } from 'react-icons/tb';
import styles from './admin.module.css';

export default function AdminPage() {
  return (
    <main className={styles.page}>
      <h1 className={styles.heading}>Admin</h1>
      <p className={styles.sub}>Manage site content</p>

      <div className={styles.grid}>
        <Link href="/admin/skills" className={styles.card}>
          <TbCode size={28} className={styles.cardIcon} />
          <span className={styles.cardLabel}>Skills</span>
        </Link>
        <Link href="/admin/projects" className={styles.card}>
          <TbDatabase size={28} className={styles.cardIcon} />
          <span className={styles.cardLabel}>Projects</span>
        </Link>
      </div>
    </main>
  );
}
