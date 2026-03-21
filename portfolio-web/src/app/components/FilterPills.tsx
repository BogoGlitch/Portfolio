'use client';

import { useRouter } from 'next/navigation';
import styles from './FilterPills.module.css';

type FilterItem = { id: number; name: string };

type FilterPillsProps = {
  technologies: FilterItem[];
  selectedIds: string[];
  basePath: string;
  paramName: string;
};

export default function FilterPills({ technologies, selectedIds, basePath, paramName }: FilterPillsProps) {
  const router = useRouter();

  const toggle = (id: number) => {
    const strId = String(id);
    const next = selectedIds.includes(strId)
      ? selectedIds.filter((s) => s !== strId)
      : [...selectedIds, strId];

    const url = next.length > 0
      ? `${basePath}?${paramName}=${next.join(',')}`
      : basePath;

    router.push(url);
  };

  return (
    <div className={styles.wrapper}>
      <span className={styles.label}>Filter</span>
      <div className={styles.pills}>
        {technologies.map((t) => {
          const active = selectedIds.includes(String(t.id));
          return (
            <button
              key={t.id}
              className={`${styles.pill} ${active ? styles.active : ''}`}
              onClick={() => toggle(t.id)}
            >
              {t.name}
            </button>
          );
        })}
      </div>
      {selectedIds.length > 0 && (
        <button className={styles.clear} onClick={() => router.push(basePath)}>
          Clear
        </button>
      )}
    </div>
  );
}
