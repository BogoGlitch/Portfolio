import type { Metadata } from "next";
import { getProjectBySlug } from "@/lib/api";
import Link from "next/link";
import { notFound } from "next/navigation";
import { TbBrandGithub, TbExternalLink } from "react-icons/tb";
import GlassCard from "@/app/components/GlassCard";
import TechTag from "@/app/components/TechTag";
import GlowButton from "@/app/components/GlowButton";
import AnimatedSection from "@/app/components/AnimatedSection";
import styles from "./page.module.css";

export async function generateMetadata({ params }: { params: Promise<{ slug: string }> }): Promise<Metadata> {
  const { slug } = await params;
  try {
    const project = await getProjectBySlug(slug);
    return {
      title: project.name,
      description: project.shortDescription ?? "Portfolio project",
    };
  } catch {
    return { title: "Project Not Found" };
  }
}

export default async function ProjectDetailPage({ params }: { params: Promise<{ slug: string }> }) {
  const { slug } = await params;

  let project;
  try {
    project = await getProjectBySlug(slug);
  } catch {
    notFound();
  }

  return (
    <div className={styles.page}>
      {/* Header band */}
      <div className={styles.header}>
        <div className={styles.headerInner}>
          <Link href="/projects" className={styles.backLink}>← Projects</Link>
          <div className={styles.titleRow}>
            <h1 className={styles.title}>{project.name}</h1>
            {project.isFeatured && <span className={styles.featuredBadge}>Featured</span>}
          </div>
          {project.shortDescription && (
            <p className={styles.subtitle}>{project.shortDescription}</p>
          )}
          {project.skills.length > 0 && (
            <div className={styles.tags}>
              {project.skills.map((s) => (
                <TechTag key={s.id} name={s.name} slug={s.slug} />
              ))}
            </div>
          )}
        </div>
      </div>

      {/* Body */}
      <div className={styles.body}>
        <div className={styles.main}>
          {project.fullDescription && (
            <AnimatedSection>
              <GlassCard>
                <h2 className={styles.sectionTitle}>About this project</h2>
                <p className={styles.bodyText}>{project.fullDescription}</p>
              </GlassCard>
            </AnimatedSection>
          )}

          {(project.repoUrl || project.liveUrl) && (
            <AnimatedSection delay={80}>
              <GlassCard>
                <h2 className={styles.sectionTitle}>Links</h2>
                <div className={styles.links}>
                  {project.repoUrl && (
                    <GlowButton href={project.repoUrl} variant="secondary" external>
                      <TbBrandGithub size={17} /> Repository
                    </GlowButton>
                  )}
                  {project.liveUrl && (
                    <GlowButton href={project.liveUrl} variant="primary" external>
                      <TbExternalLink size={17} /> Live Site
                    </GlowButton>
                  )}
                </div>
              </GlassCard>
            </AnimatedSection>
          )}
        </div>

        <aside className={styles.sidebar}>
          {project.skills.length > 0 && (
            <AnimatedSection delay={120}>
              <GlassCard>
                <h2 className={styles.sectionTitle}>Skills</h2>
                <div className={styles.techList}>
                  {project.skills.map((s) => (
                    <Link key={s.id} href={`/skills/${s.slug}`} className={styles.techItem}>
                      {s.name}
                    </Link>
                  ))}
                </div>
              </GlassCard>
            </AnimatedSection>
          )}
        </aside>
      </div>
    </div>
  );
}
