import Link from "next/link";
import { getTechnologies } from "@/lib/api";
import type { Metadata } from "next";
import GlassCard from "../components/GlassCard";
import AnimatedSection from "../components/AnimatedSection";
import DisciplineIcon from "../components/DisciplineIcon";
import CategoryIcon from "../components/CategoryIcon";
import { TbLayoutGrid } from "react-icons/tb";
import styles from "./page.module.css";

export const metadata: Metadata = {
  title: "Technologies",
  description: "Technologies, frameworks, and platform choices used across the portfolio.",
};

type TechnologiesPageProps = {
  searchParams?: Promise<{ discipline?: string }>;
};

const DISCIPLINE_ORDER = ['Frontend', 'Backend', 'Database', 'Cloud', 'DevOps', 'AI'];

const CATEGORY_ORDER = ['Language', 'Platform', 'Framework', 'Library', 'Database', 'ORM', 'Testing', 'Tool', 'Styling', 'Source Control', 'Cloud Service', 'CI/CD', 'AI Assistant'];

function categoryRank(category: string | null | undefined): number {
  const idx = CATEGORY_ORDER.indexOf(category ?? '');
  return idx === -1 ? CATEGORY_ORDER.length : idx;
}

export default async function TechnologiesPage({ searchParams }: TechnologiesPageProps) {
  const resolved = await searchParams;
  const selectedDiscipline = resolved?.discipline?.trim() ?? '';

  const technologies = await getTechnologies();

  // Primary: technologies used in projects rank first (more projects = higher)
  // Secondary: displayOrder encodes global recruiter-value ranking (lower = more valuable)
  const byValue = (a: (typeof technologies)[number], b: (typeof technologies)[number]) =>
    b.projects.length - a.projects.length || a.displayOrder - b.displayOrder;

  const filtered = selectedDiscipline
    ? [...technologies]
        .filter(t => t.discipline === selectedDiscipline)
        .sort((a, b) => categoryRank(a.category) - categoryRank(b.category) || byValue(a, b))
    : [...technologies].sort(byValue);

  const availableDisciplines = DISCIPLINE_ORDER.filter(d =>
    technologies.some(t => t.discipline === d),
  );

  return (
    <div className={styles.page}>
      <div className={styles.header}>
        <div className={styles.headerInner}>
          <Link href="/" className={styles.backLink}>← Home</Link>
          <h1 className={styles.title}>Technologies</h1>
          <p className={styles.subtitle}>
            Explore the tools and platforms used across the portfolio.
          </p>
        </div>
      </div>

      {/* Discipline filter bar */}
      <div className={styles.filterBand}>
        <div className={styles.filterBandInner}>
          <Link
            href="/technologies"
            className={`${styles.disciplinePill} ${!selectedDiscipline ? styles.disciplinePillActive : ''}`}
          >
            <TbLayoutGrid size={14} aria-hidden="true" />
            All
          </Link>
          {availableDisciplines.map(d => (
            <Link
              key={d}
              href={selectedDiscipline === d ? '/technologies' : `/technologies?discipline=${encodeURIComponent(d)}`}
              className={`${styles.disciplinePill} ${selectedDiscipline === d ? styles.disciplinePillActive : ''}`}
            >
              <DisciplineIcon discipline={d} size={14} />
              {d}
            </Link>
          ))}
        </div>
      </div>

      <div className={styles.body}>
        <div className={styles.grid}>
          {filtered.map((tech, i) => (
            <AnimatedSection key={tech.id} delay={i * 50}>
              <GlassCard href={`/technologies/${tech.slug}`}>
                <div className={styles.cardInner}>
                  <div className={styles.cardTop}>
                    <div className={styles.cardHeader}>
                      <CategoryIcon category={tech.category} size={24} className={styles.cardIcon} />
                      {tech.category && (
                        <span className={styles.category}>{tech.category}</span>
                      )}
                    </div>
                    <h2 className={styles.cardTitle}>{tech.name}</h2>
                    {tech.description && (
                      <p className={styles.cardDesc}>{tech.description}</p>
                    )}
                  </div>
                  <div className={styles.cardFooter}>
                    <span className={styles.projectCount}>
                      {tech.projects.length} {tech.projects.length === 1 ? 'project' : 'projects'}
                    </span>
                  </div>
                </div>
              </GlassCard>
            </AnimatedSection>
          ))}
        </div>

        {filtered.length === 0 && (
          <div className={styles.empty}>
            <p>No technologies in this discipline yet.</p>
          </div>
        )}
      </div>
    </div>
  );
}
