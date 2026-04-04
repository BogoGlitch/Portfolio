import GlowButton from "@/app/components/GlowButton";
import styles from "./not-found.module.css";

export default function SkillNotFound() {
  return (
    <div className={styles.page}>
      <p className={styles.code}>404</p>
      <h1 className={styles.title}>Skill not found</h1>
      <p className={styles.body}>The skill you&apos;re looking for doesn&apos;t exist or may have been removed.</p>
      <GlowButton href="/skills" variant="secondary">← Back to Skills</GlowButton>
    </div>
  );
}
