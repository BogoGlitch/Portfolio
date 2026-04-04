import type { Metadata } from "next";
import Link from "next/link";
import { getSkillBySlug } from "@/lib/api";
import { notFound } from "next/navigation";
import { TbBook } from "react-icons/tb";
import GlassCard from "@/app/components/GlassCard";
import GlowButton from "@/app/components/GlowButton";
import AnimatedSection from "@/app/components/AnimatedSection";
import TechIcon from "@/app/components/TechIcon";
import styles from "./page.module.css";

export async function generateMetadata({ params }: { params: Promise<{ slug: string }> }): Promise<Metadata> {
  const { slug } = await params;
  try {
    const skill = await getSkillBySlug(slug);
    return { title: skill.name, description: skill.description ?? "Skill details" };
  } catch {
    return { title: "Skill Not Found" };
  }
}

export default async function SkillDetailPage({ params }: { params: Promise<{ slug: string }> }) {
  const { slug } = await params;

  let skill;
  try {
    skill = await getSkillBySlug(slug);
  } catch {
    notFound();
  }

  return (
    <div className={styles.page}>
      <div className={styles.header}>
        <div className={styles.headerInner}>
          <Link href="/skills" className={styles.backLink}>← Skills</Link>
          <div className={styles.titleRow}>
            <TechIcon slug={skill.slug} size={36} className={styles.titleIcon} />
            <h1 className={styles.title}>{skill.name}</h1>
            {skill.category && (
              <span className={styles.category}>{skill.category}</span>
            )}
          </div>
          {skill.description && (
            <p className={styles.subtitle}>{skill.description}</p>
          )}
          {skill.documentationUrl && (
            <GlowButton href={skill.documentationUrl} variant="ghost" external>
              <TbBook size={16} /> Documentation
            </GlowButton>
          )}
        </div>
      </div>

      <div className={styles.body}>
        <AnimatedSection>
          <GlassCard>
            <h2 className={styles.sectionTitle}>
              Used in {skill.projects.length} {skill.projects.length === 1 ? 'project' : 'projects'}
            </h2>

            {skill.projects.length === 0 ? (
              <p className={styles.empty}>No projects linked to this skill yet.</p>
            ) : (
              <div className={styles.projectGrid}>
                {skill.projects.map((project) => (
                  <Link key={project.id} href={`/projects/${project.slug}`} className={styles.projectItem}>
                    <span className={styles.projectName}>{project.name}</span>
                    <span className={styles.projectArrow}>→</span>
                  </Link>
                ))}
              </div>
            )}
          </GlassCard>
        </AnimatedSection>
      </div>
    </div>
  );
}
