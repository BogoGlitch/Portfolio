import styles from './SkeletonLoader.module.css';

type SkeletonLoaderProps = {
  variant?: 'card' | 'detail' | 'list';
  count?: number;
};

function SkeletonCard() {
  return (
    <div className={styles.card}>
      <div className={`skeleton ${styles.image}`} />
      <div className={styles.body}>
        <div className={`skeleton ${styles.title}`} />
        <div className={`skeleton ${styles.text}`} />
        <div className={`skeleton ${styles.textShort}`} />
        <div className={styles.tags}>
          <div className={`skeleton ${styles.tag}`} />
          <div className={`skeleton ${styles.tag}`} />
          <div className={`skeleton ${styles.tag}`} />
        </div>
      </div>
    </div>
  );
}

function SkeletonDetail() {
  return (
    <div className={styles.detail}>
      <div className={`skeleton ${styles.detailTitle}`} />
      <div className={`skeleton ${styles.detailText}`} />
      <div className={`skeleton ${styles.detailText}`} />
      <div className={`skeleton ${styles.detailTextShort}`} />
    </div>
  );
}

export default function SkeletonLoader({ variant = 'card', count = 3 }: SkeletonLoaderProps) {
  if (variant === 'detail') return <SkeletonDetail />;

  return (
    <div className={styles.grid}>
      {Array.from({ length: count }).map((_, i) => (
        <SkeletonCard key={i} />
      ))}
    </div>
  );
}
