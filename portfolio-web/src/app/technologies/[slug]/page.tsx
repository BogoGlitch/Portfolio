import type { Metadata } from "next";
import Link from "next/link";
import { getTechnologyBySlug } from "@/lib/api";
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
    const tech = await getTechnologyBySlug(slug);
    return { title: tech.name, description: tech.description ?? "Technology details" };
  } catch {
    return { title: "Technology Not Found" };
  }
}

export default async function TechnologyDetailPage({ params }: { params: Promise<{ slug: string }> }) {
  const { slug } = await params;

  let technology;
  try {
    technology = await getTechnologyBySlug(slug);
  } catch {
    notFound();
  }

  return (
    <div className={styles.page}>
      <div className={styles.header}>
        <div className={styles.headerInner}>
          <Link href="/technologies" className={styles.backLink}>← Technologies</Link>
          <div className={styles.titleRow}>
            <TechIcon slug={technology.slug} size={36} className={styles.titleIcon} />
            <h1 className={styles.title}>{technology.name}</h1>
            {technology.category && (
              <span className={styles.category}>{technology.category}</span>
            )}
          </div>
          {technology.description && (
            <p className={styles.subtitle}>{technology.description}</p>
          )}
          {technology.documentationUrl && (
            <GlowButton href={technology.documentationUrl} variant="ghost" external>
              <TbBook size={16} /> Documentation
            </GlowButton>
          )}
        </div>
      </div>

      <div className={styles.body}>
        <AnimatedSection>
          <GlassCard>
            <h2 className={styles.sectionTitle}>
              Used in {technology.projects.length} {technology.projects.length === 1 ? 'project' : 'projects'}
            </h2>

            {technology.projects.length === 0 ? (
              <p className={styles.empty}>No projects linked to this technology yet.</p>
            ) : (
              <div className={styles.projectGrid}>
                {technology.projects.map((project) => (
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
