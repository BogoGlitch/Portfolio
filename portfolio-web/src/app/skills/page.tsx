import Link from "next/link";
import { getSkills } from "@/lib/api";
import type { Metadata } from "next";
import GlassCard from "../components/GlassCard";
import AnimatedSection from "../components/AnimatedSection";
import DisciplineIcon from "../components/DisciplineIcon";
import CategoryIcon from "../components/CategoryIcon";
import { TbLayoutGrid } from "react-icons/tb";
import styles from "./page.module.css";

export const metadata: Metadata = {
  title: "Skills",
  description: "Skills, frameworks, and platform choices used across the portfolio.",
};

type SkillsPageProps = {
  searchParams?: Promise<{ discipline?: string }>;
};

const DISCIPLINE_ORDER = ['Frontend', 'Backend', 'Database', 'Cloud', 'DevOps', 'AI'];

const CATEGORY_ORDER = ['Language', 'Platform', 'Framework', 'Library', 'Database', 'ORM', 'Testing', 'Tool', 'Styling', 'Source Control', 'Cloud Service', 'CI/CD', 'AI Assistant'];

function categoryRank(category: string | null | undefined): number {
  const idx = CATEGORY_ORDER.indexOf(category ?? '');
  return idx === -1 ? CATEGORY_ORDER.length : idx;
}

export default async function SkillsPage({ searchParams }: SkillsPageProps) {
  const resolved = await searchParams;
  const selectedDiscipline = resolved?.discipline?.trim() ?? '';

  let skills: Awaited<ReturnType<typeof getSkills>>;
  try {
    skills = await getSkills();
  } catch {
    skills = [];
  }

  // Primary: skills used in projects rank first (more projects = higher)
  // Secondary: displayOrder encodes global recruiter-value ranking (lower = more valuable)
  const byValue = (a: (typeof skills)[number], b: (typeof skills)[number]) =>
    b.projects.length - a.projects.length || a.displayOrder - b.displayOrder;

  const filtered = selectedDiscipline
    ? [...skills]
        .filter(s => s.discipline === selectedDiscipline)
        .sort((a, b) => categoryRank(a.category) - categoryRank(b.category) || byValue(a, b))
    : [...skills].sort(byValue);

  const availableDisciplines = DISCIPLINE_ORDER.filter(d =>
    skills.some(s => s.discipline === d),
  );

  return (
    <div className={styles.page}>
      <div className={styles.header}>
        <div className={styles.headerInner}>
          <Link href="/" className={styles.backLink}>← Home</Link>
          <h1 className={styles.title}>Skills</h1>
          <p className={styles.subtitle}>
            Explore the tools and platforms used across the portfolio.
          </p>
        </div>
      </div>

      {/* Discipline filter bar */}
      <div className={styles.filterBand}>
        <div className={styles.filterBandInner}>
          <Link
            href="/skills"
            className={`${styles.disciplinePill} ${!selectedDiscipline ? styles.disciplinePillActive : ''}`}
          >
            <TbLayoutGrid size={14} aria-hidden="true" />
            All
          </Link>
          {availableDisciplines.map(d => (
            <Link
              key={d}
              href={selectedDiscipline === d ? '/skills' : `/skills?discipline=${encodeURIComponent(d)}`}
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
          {filtered.map((skill, i) => (
            <AnimatedSection key={skill.id} delay={i * 25} instant={i < 5}>
              <GlassCard href={`/skills/${skill.slug}`}>
                <div className={styles.cardInner}>
                  <div className={styles.cardTop}>
                    <div className={styles.cardHeader}>
                      <CategoryIcon category={skill.category} size={24} className={styles.cardIcon} />
                      {skill.category && (
                        <span className={styles.category}>{skill.category}</span>
                      )}
                    </div>
                    <h2 className={styles.cardTitle}>{skill.name}</h2>
                    {skill.description && (
                      <p className={styles.cardDesc}>{skill.description}</p>
                    )}
                  </div>
                  <div className={styles.cardFooter}>
                    <span className={styles.projectCount}>
                      {skill.projects.length} {skill.projects.length === 1 ? 'project' : 'projects'}
                    </span>
                  </div>
                </div>
              </GlassCard>
            </AnimatedSection>
          ))}
        </div>

        {filtered.length === 0 && (
          <div className={styles.empty}>
            <p>No skills in this discipline yet.</p>
          </div>
        )}
      </div>
    </div>
  );
}
