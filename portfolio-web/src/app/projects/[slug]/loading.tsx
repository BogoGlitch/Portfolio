import SkeletonLoader from "@/app/components/SkeletonLoader";
import styles from "./loading.module.css";

export default function LoadingProjectDetail() {
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
        <SkeletonLoader variant="detail" />
      </div>
    </div>
  );
}
