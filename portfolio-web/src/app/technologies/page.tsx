import Link from "next/link";
import { getTechnologies } from "@/lib/api";
import type { Metadata } from "next";
import GlassCard from "../components/GlassCard";
import AnimatedSection from "../components/AnimatedSection";
import CategoryTabs from "../components/CategoryTabs";
import TechIcon from "../components/TechIcon";
import styles from "./page.module.css";

export const metadata: Metadata = {
  title: "Technologies",
  description: "Technologies, frameworks, and platform choices used across the portfolio.",
};

type TechnologiesPageProps = {
  searchParams?: Promise<{ category?: string | string[] }>;
};

function parseCategory(value: string | string[] | undefined): string | undefined {
  const raw = Array.isArray(value) ? value[0] : value;
  return raw?.trim() || undefined;
}

export default async function TechnologiesPage({ searchParams }: TechnologiesPageProps) {
  const resolved = await searchParams;
  const selectedCategory = parseCategory(resolved?.category);

  const technologies = await getTechnologies();

  const categories = Array.from(
    new Set(technologies.map((t) => t.category).filter((c): c is string => Boolean(c))),
  ).sort();

  const filtered = selectedCategory
    ? technologies.filter((t) => t.category === selectedCategory)
    : technologies;

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
        <CategoryTabs
          categories={categories}
          selected={selectedCategory}
          basePath="/technologies"
          paramName="category"
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
            <p>No technologies in this category.</p>
          </div>
        )}
      </div>
    </div>
  );
}
