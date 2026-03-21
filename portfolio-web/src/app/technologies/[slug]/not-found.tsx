import GlowButton from "@/app/components/GlowButton";
import styles from "./not-found.module.css";

export default function TechnologyNotFound() {
  return (
    <div className={styles.page}>
      <p className={styles.code}>404</p>
      <h1 className={styles.title}>Technology not found</h1>
      <p className={styles.body}>The technology you&apos;re looking for doesn&apos;t exist or may have been removed.</p>
      <GlowButton href="/technologies" variant="secondary">← Back to Technologies</GlowButton>
    </div>
  );
}
