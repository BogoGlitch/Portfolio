import { getProjects, getTechnologies } from "@/lib/api";
import Link from "next/link";
import Image from "next/image";
import type { Metadata } from "next";
import GlassCard from "../components/GlassCard";
import TechTag from "../components/TechTag";
import AnimatedSection from "../components/AnimatedSection";
import GlowButton from "../components/GlowButton";
import ProjectFilterModal from "../components/ProjectFilterModal";
import styles from "./page.module.css";

export const metadata: Metadata = {
  title: "Projects",
  description: "Portfolio projects highlighting backend design, API architecture, and end-to-end implementation decisions.",
};

type ProjectsPageProps = {
  searchParams?: Promise<{ technologyIds?: string }>;
};

export default async function ProjectsPage({ searchParams }: ProjectsPageProps) {
  const resolved = await searchParams;
  const selectedTechIds = resolved?.technologyIds
    ? resolved.technologyIds.split(',').map(s => s.trim()).filter(Boolean)
    : [];

  const [projects, technologies] = await Promise.all([
    getProjects({ technologyIds: selectedTechIds }),
    getTechnologies(),
  ]);

  // Featured projects rise to the top, then sort by displayOrder
  const sorted = [...projects].sort((a, b) => {
    if (a.isFeatured !== b.isFeatured) return a.isFeatured ? -1 : 1;
    return a.displayOrder - b.displayOrder;
  });

  return (
    <div className={styles.page}>
      <div className={styles.header}>
        <div className={styles.headerInner}>
          <Link href="/" className={styles.backLink}>← Home</Link>
          <h1 className={styles.title}>Projects</h1>
          <p className={styles.subtitle}>
            Browse portfolio projects and filter by technology.
          </p>
        </div>
      </div>

      <div className={styles.body}>
        <ProjectFilterModal
          technologies={technologies}
          currentTechIds={selectedTechIds}
          basePath="/projects"
        />

        {sorted.length === 0 ? (
          <div className={styles.empty}>
            <p>No projects matched the selected filters.</p>
            <GlowButton href="/projects" variant="ghost">Clear filters</GlowButton>
          </div>
        ) : (
          <div className={styles.grid}>
            {sorted.map((project, i) => (
              <AnimatedSection
                key={project.id}
                delay={i * 60}
                className={project.isFeatured ? styles.featuredItem : ''}
              >
                <GlassCard href={`/projects/${project.slug}`} featured={project.isFeatured}>
                  {project.isFeatured ? (
                    /* Featured — full-width horizontal layout */
                    <div className={styles.featuredCardInner}>
                      <div className={styles.cardImageWrap}>
                        <Image
                          src={project.imageUrl ?? `https://picsum.photos/seed/${project.id}/800/450`}
                          alt={`${project.name} screenshot`}
                          fill
                          className={styles.cardImage}
                        />
                      </div>
                      <div className={styles.cardContent}>
                        <div className={styles.cardTop}>
                          <span className={styles.featuredBadge}>Featured</span>
                          <h2 className={styles.cardTitle}>{project.name}</h2>
                          <p className={styles.cardDesc}>{project.shortDescription}</p>
                        </div>
                        {project.technologies.length > 0 && (
                          <div className={styles.cardTags}>
                            {project.technologies.slice(0, 6).map((t) => (
                              <TechTag key={t.id} name={t.name} />
                            ))}
                            {project.technologies.length > 6 && (
                              <span className={styles.moreTags}>+{project.technologies.length - 6}</span>
                            )}
                          </div>
                        )}
                      </div>
                    </div>
                  ) : (
                    /* Regular — vertical layout with image on top */
                    <div className={styles.regularCardInner}>
                      <div className={styles.cardImageWrap}>
                        <Image
                          src={project.imageUrl ?? `https://picsum.photos/seed/${project.id}/800/450`}
                          alt={`${project.name} screenshot`}
                          fill
                          className={styles.cardImage}
                        />
                      </div>
                      <div className={styles.cardContent}>
                        <div className={styles.cardTop}>
                          <h2 className={styles.cardTitle}>{project.name}</h2>
                          <p className={styles.cardDesc}>{project.shortDescription}</p>
                        </div>
                        {project.technologies.length > 0 && (
                          <div className={styles.cardTags}>
                            {project.technologies.slice(0, 4).map((t) => (
                              <TechTag key={t.id} name={t.name} />
                            ))}
                            {project.technologies.length > 4 && (
                              <span className={styles.moreTags}>+{project.technologies.length - 4}</span>
                            )}
                          </div>
                        )}
                      </div>
                    </div>
                  )}
                </GlassCard>
              </AnimatedSection>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
