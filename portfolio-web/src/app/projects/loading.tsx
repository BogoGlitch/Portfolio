import styles from './loading.module.css';

export default function LoadingProjects() {
  return (
    <div className={styles.page}>
      <div className={styles.header}>
        <div className={styles.inner}>
          <div className={`skeleton ${styles.backLink}`} />
          <div className={`skeleton ${styles.title}`} />
          <div className={`skeleton ${styles.subtitle}`} />
        </div>
      </div>

      <div className={styles.body}>
        <div className={`skeleton ${styles.filterBtn}`} />

        <div className={styles.grid}>
          {/* Featured card placeholder */}
          <div className={styles.featuredItem}>
            <div className={styles.featuredCard}>
              <div className={`skeleton ${styles.featuredImage}`} />
              <div className={styles.cardContent}>
                <div className={`skeleton ${styles.badge}`} />
                <div className={`skeleton ${styles.cardTitle}`} />
                <div className={`skeleton ${styles.cardText}`} />
                <div className={`skeleton ${styles.cardText}`} style={{ width: '80%' }} />
                <div className={styles.tags}>
                  <div className={`skeleton ${styles.tag}`} />
                  <div className={`skeleton ${styles.tag}`} />
                  <div className={`skeleton ${styles.tag}`} />
                </div>
              </div>
            </div>
          </div>

          {/* Regular card placeholders */}
          {Array.from({ length: 4 }).map((_, i) => (
            <div key={i} className={styles.card}>
              <div className={`skeleton ${styles.cardImage}`} />
              <div className={styles.cardContent}>
                <div className={`skeleton ${styles.cardTitle}`} />
                <div className={`skeleton ${styles.cardText}`} />
                <div className={`skeleton ${styles.cardText}`} style={{ width: '75%' }} />
                <div className={styles.tags}>
                  <div className={`skeleton ${styles.tag}`} />
                  <div className={`skeleton ${styles.tag}`} />
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
