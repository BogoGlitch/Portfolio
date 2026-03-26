import Link from "next/link";
import { getTechnologies } from "@/lib/api";
import type { Metadata } from "next";
import GlassCard from "../components/GlassCard";
import AnimatedSection from "../components/AnimatedSection";
import FilterModal, { type FilterGroup } from "../components/FilterModal";
import TechIcon from "../components/TechIcon";
import styles from "./page.module.css";

export const metadata: Metadata = {
  title: "Technologies",
  description: "Technologies, frameworks, and platform choices used across the portfolio.",
};

type TechnologiesPageProps = {
  searchParams?: Promise<{ discipline?: string; category?: string }>;
};

const DISCIPLINE_ORDER = ['Frontend', 'Backend', 'Database', 'Cloud', 'DevOps', 'AI'];

export default async function TechnologiesPage({ searchParams }: TechnologiesPageProps) {
  const resolved = await searchParams;
  const selectedDisciplines = resolved?.discipline
    ? resolved.discipline.split(',').map(s => s.trim()).filter(Boolean)
    : [];
  const selectedCategories = resolved?.category
    ? resolved.category.split(',').map(s => s.trim()).filter(Boolean)
    : [];

  const technologies = await getTechnologies();

  const filtered = technologies.filter(t => {
    const matchesDiscipline = selectedDisciplines.length === 0 || selectedDisciplines.includes(t.discipline ?? '');
    const matchesCategory = selectedCategories.length === 0 || selectedCategories.includes(t.category ?? '');
    return matchesDiscipline && matchesCategory;
  });

  // Derive ordered discipline list from data
  const disciplines = DISCIPLINE_ORDER.filter(d => technologies.some(t => t.discipline === d));

  // Derive sorted category list from data
  const categories = Array.from(
    new Set(technologies.map((t) => t.category).filter((c): c is string => Boolean(c))),
  ).sort();

  const filterGroups: FilterGroup[] = [
    {
      label: 'Discipline',
      paramName: 'discipline',
      multiSelect: true,
      items: disciplines.map(d => ({ value: d, label: d })),
    },
    {
      label: 'Category',
      paramName: 'category',
      multiSelect: true,
      items: categories.map(c => ({ value: c, label: c })),
    },
  ];

  const filterCurrent: Record<string, string[]> = {
    discipline: selectedDisciplines,
    category: selectedCategories,
  };

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

      <div className={styles.body}>
        <FilterModal
          groups={filterGroups}
          current={filterCurrent}
          basePath="/technologies"
        />

        <div className={styles.grid}>
          {filtered.map((tech, i) => (
            <AnimatedSection key={tech.id} delay={i * 50}>
              <GlassCard href={`/technologies/${tech.slug}`} featured={tech.isFeatured}>
                <div className={styles.cardInner}>
                  <div className={styles.cardTop}>
                    <div className={styles.cardHeader}>
                      <TechIcon slug={tech.slug} size={24} className={styles.cardIcon} />
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
            <p>No technologies matched the selected filters.</p>
          </div>
        )}
      </div>
    </div>
  );
}
