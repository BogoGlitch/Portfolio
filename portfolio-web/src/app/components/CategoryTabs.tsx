'use client';

import { useRouter } from 'next/navigation';
import styles from './CategoryTabs.module.css';

type CategoryTabsProps = {
  categories: string[];
  selected: string | undefined;
  basePath: string;
  paramName: string;
};

export default function CategoryTabs({ categories, selected, basePath, paramName }: CategoryTabsProps) {
  const router = useRouter();

  const navigate = (category: string | undefined) => {
    const url = category ? `${basePath}?${paramName}=${encodeURIComponent(category)}` : basePath;
    router.push(url);
  };

  return (
    <div className={styles.tabs}>
      <button
        className={`${styles.tab} ${!selected ? styles.active : ''}`}
        onClick={() => navigate(undefined)}
      >
        All
      </button>
      {categories.map((cat) => (
        <button
          key={cat}
          className={`${styles.tab} ${selected === cat ? styles.active : ''}`}
          onClick={() => navigate(cat)}
        >
          {cat}
        </button>
      ))}
    </div>
  );
}
