import styles from './SkeletonLoader.module.css';

// ── Primitives ────────────────────────────────────────────────────────────────

/** Single shimmer block. Use anywhere you need a one-off skeleton element. */
export function Skeleton({
  height = '1rem',
  width = '100%',
  borderRadius,
}: {
  height?: string;
  width?: string;
  borderRadius?: string;
}) {
  return <div className="skeleton" style={{ height, width, borderRadius }} />;
}

/**
 * Skeleton rows for use inside an existing <tbody>.
 * Renders `rows` shimmer rows with `cols - 1` shimmer cells + one empty
 * actions cell, so the layout matches admin tables exactly.
 *
 * Cell widths taper naturally from wide (name) to narrow (flags).
 */
export function SkeletonTableRows({
  rows = 6,
  cols,
}: {
  rows?: number;
  cols: number;
}) {
  const widths = ['44%', '24%', '16%', '12%', '8%', '6%', '5%'];
  return (
    <>
      {Array.from({ length: rows }).map((_, r) => (
        <tr key={r}>
          {Array.from({ length: cols - 1 }).map((_, c) => (
            <td key={c}>
              <div className={`skeleton ${styles.cellBar}`} style={{ width: widths[c] ?? '10%' }} />
            </td>
          ))}
          <td /> {/* actions column */}
        </tr>
      ))}
    </>
  );
}

// ── Composed skeletons ────────────────────────────────────────────────────────

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
