import GlowButton from "@/app/components/GlowButton";
import styles from "./not-found.module.css";

export default function ProjectNotFound() {
  return (
    <div className={styles.page}>
      <p className={styles.code}>404</p>
      <h1 className={styles.title}>Project not found</h1>
      <p className={styles.body}>The project you&apos;re looking for doesn&apos;t exist or may have been removed.</p>
      <GlowButton href="/projects" variant="secondary">← Back to Projects</GlowButton>
    </div>
  );
}
