import styles from './loading.module.css';

// Approximate the real discipline pill bar so the layout doesn't shift on load.
// These widths loosely match: All, Frontend, Backend, Database, Cloud, DevOps, AI
const PILL_WIDTHS = ['3.5rem', '5.5rem', '5rem', '5.5rem', '4rem', '5rem', '3rem'];

export default function LoadingSkills() {
  return (
    <div className={styles.page}>
      <div className={styles.header}>
        <div className={styles.inner}>
          <div className={`skeleton ${styles.backLink}`} />
          <div className={`skeleton ${styles.title}`} />
          <div className={`skeleton ${styles.subtitle}`} />
        </div>
      </div>

      <div className={styles.filterBand}>
        <div className={styles.filterBandInner}>
          {PILL_WIDTHS.map((w, i) => (
            <div key={i} className={`skeleton ${styles.pill}`} style={{ width: w }} />
          ))}
        </div>
      </div>

      <div className={styles.body}>
        <div className={styles.grid}>
          {Array.from({ length: 12 }).map((_, i) => (
            <div key={i} className={styles.card}>
              <div className={styles.cardHeader}>
                <div className={`skeleton ${styles.cardIcon}`} />
                <div className={`skeleton ${styles.cardCategory}`} />
              </div>
              <div className={`skeleton ${styles.cardTitle}`} />
              <div className={`skeleton ${styles.cardText}`} />
              <div className={`skeleton ${styles.cardText}`} style={{ width: '80%' }} />
              <div className={styles.cardFooter}>
                <div className={`skeleton ${styles.cardCount}`} />
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
