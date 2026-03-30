"use client";

import styles from "./error.module.css";

export default function GlobalError({
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  return (
    <div className={styles.page}>
      <div className={styles.card}>
        <h2 className={styles.title}>Something went wrong</h2>
        <p className={styles.message}>
          The page couldn&apos;t load right now. This usually resolves in a few seconds.
        </p>
        <button onClick={() => reset()} className={styles.button}>
          Try again
        </button>
      </div>
    </div>
  );
}
