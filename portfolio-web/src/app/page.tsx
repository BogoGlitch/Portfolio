import Image from "next/image";
import ImageWithSkeleton from "./components/ImageWithSkeleton";
import Link from "next/link";
import type { Metadata } from "next";
import { getProjects, getSkills } from "@/lib/api";
import { TbFolder, TbCpu, TbBrain } from "react-icons/tb";
import GlassCard from "./components/GlassCard";
import GlowButton from "./components/GlowButton";
import AnimatedSection from "./components/AnimatedSection";
import TechTag from "./components/TechTag";
import HeroTypewriter from "./components/HeroTypewriter";
import styles from "./page.module.css";

export const metadata: Metadata = {
  title: "Home | Sean Bogolin",
  description:
    "Backend-first portfolio platform showcasing projects, skills, and pragmatic software engineering decisions.",
};

export default async function HomePage() {
  const [projects, skills] = await Promise.allSettled([
    getProjects(),
    getSkills(),
  ]);

  const projectCount = projects.status === "fulfilled" ? projects.value.length : 0;
  const skillCount = skills.status === "fulfilled" ? skills.value.length : 0;
  const featuredProjects = projects.status === "fulfilled"
    ? projects.value.filter((p) => p.isFeatured).slice(0, 3)
    : [];

  return (
    <div className={styles.page}>
      {/* ── Hero ── */}
      <section className={styles.hero}>
        {/* Banner — two images, CSS shows the correct one per theme */}
        <Image
          src="/images/home-hero-banner.png"
          alt=""
          role="presentation"
          fill
          priority
          sizes="100vw"
          className={`${styles.heroBgImage} ${styles.heroBgDark}`}
        />
        <Image
          src="/images/prism-banner.jpg"
          alt=""
          role="presentation"
          fill
          sizes="100vw"
          className={`${styles.heroBgImage} ${styles.heroBgLight}`}
        />
        {/* Gradient overlay — darkens the banner for text legibility */}
        <div className={styles.heroBgOverlay} aria-hidden="true" />

        <div className={styles.heroInner}>
          <div className={styles.heroText}>
            <p className={styles.kicker}>Senior Software Engineer</p>
            <h1 className={styles.name}>Sean Bogolin</h1>
            <p className={styles.typewriterLine}>
              <HeroTypewriter />
            </p>
            <p className={styles.description}>
              I design and build scalable, maintainable web platforms with a strong focus on
              backend architecture, API design, and end-to-end delivery.
            </p>
            <div className={styles.heroActions}>
              <GlowButton href="/projects" variant="primary">View Projects</GlowButton>
              <GlowButton href="/skills" variant="secondary">Browse Skills</GlowButton>
            </div>
          </div>

          <div className={styles.heroMedia}>
            <ImageWithSkeleton
              src="/images/headshot.jpg"
              alt="Portrait of Sean Bogolin"
              fill
              sizes="264px"
              priority
              className={styles.avatarWrapper}
              imgClassName={styles.avatar}
            />
          </div>
        </div>

        {/* Stats bar */}
        <div className={styles.statsBar}>
          <div className={styles.statsInner}>
            <div className={styles.stat}>
              <span className={styles.statValue}>{projectCount}</span>
              <span className={styles.statLabel}>Projects</span>
            </div>
            <div className={styles.statDivider} />
            <div className={styles.stat}>
              <span className={styles.statValue}>{skillCount}</span>
              <span className={styles.statLabel}>Skills</span>
            </div>
            <div className={styles.statDivider} />
            <div className={styles.stat}>
              <span className={styles.statValue}>.NET 10</span>
              <span className={styles.statLabel}>Backend</span>
            </div>
            <div className={styles.statDivider} />
            <div className={styles.stat}>
              <span className={styles.statValue}>Next.js 16</span>
              <span className={styles.statLabel}>Frontend</span>
            </div>
          </div>
        </div>
      </section>

      {/* ── What this demonstrates ── constrained, no band, no outer animation */}
      <div className={styles.content}>
        <section className={styles.section}>
          <h2 className={styles.sectionTitle}>What this portfolio demonstrates</h2>
          <p className={styles.sectionSubtitle}>
            More than finished screens — this platform highlights how I think through backend
            architecture, API contracts, data modeling, and pragmatic delivery.
          </p>
          <div className={styles.highlightGrid}>
            {[
              { title: "Backend-first design", body: "Clear service boundaries, DTO-driven APIs, and data access shaped for maintainability." },
              { title: "Pragmatic architecture", body: "Decisions grounded in tradeoffs — scalability, readability, and delivery maturity." },
              { title: "End-to-end ownership", body: "Database and API design through frontend integration and deployment readiness." },
              { title: "Enterprise mindset", body: "Structured patterns, explicit configuration, and systems built for long-term evolution." },
            ].map((item, i) => (
              <AnimatedSection key={item.title} delay={i * 40}>
                <GlassCard className={styles.highlightCard}>
                  <h3 className={styles.highlightTitle}>{item.title}</h3>
                  <p className={styles.highlightBody}>{item.body}</p>
                </GlassCard>
              </AnimatedSection>
            ))}
          </div>
        </section>
      </div>

      {/* ── Featured projects ── full-width band */}
      {featuredProjects.length > 0 && (
        <AnimatedSection as="section" className={styles.featuredBand}>
          <div className={styles.bandInner}>
            <div className={styles.sectionHeader}>
              <h2 className={styles.sectionTitle}>Featured projects</h2>
              <Link href="/projects" className={styles.sectionLink}>View all →</Link>
            </div>
            <div className={styles.featuredGrid}>
              {featuredProjects.map((project, i) => (
                <GlassCard key={project.id} href={`/projects/${project.slug}`}>
                  {i === 0 ? (
                    /* Wide hero card — image + content side by side */
                    <div className={styles.featuredHeroCard}>
                      <ImageWithSkeleton
                        src={project.imageUrl ?? `https://picsum.photos/seed/${project.id}/800/450`}
                        alt={`${project.name} screenshot`}
                        fill
                        className={styles.projectImageWrap}
                        imgClassName={styles.projectImage}
                      />
                      <div className={styles.projectMeta}>
                        <h3 className={styles.projectTitle}>{project.name}</h3>
                        <p className={styles.projectDesc}>{project.shortDescription}</p>
                        {project.skills.length > 0 && (
                          <div className={styles.projectTags}>
                            {project.skills.slice(0, 6).map((s) => (
                              <TechTag key={s.id} name={s.name} />
                            ))}
                          </div>
                        )}
                      </div>
                    </div>
                  ) : (
                    /* Smaller cards — image left on wide, image top on narrow */
                    <div className={styles.projectCard}>
                      <ImageWithSkeleton
                        src={project.imageUrl ?? `https://picsum.photos/seed/${project.id}/800/450`}
                        alt={`${project.name} screenshot`}
                        fill
                        className={styles.projectCardImage}
                        imgClassName={styles.projectImage}
                      />
                      <div className={styles.projectCardContent}>
                        <div className={styles.projectMeta}>
                          <h3 className={styles.projectTitle}>{project.name}</h3>
                          <p className={styles.projectDesc}>{project.shortDescription}</p>
                        </div>
                        {project.skills.length > 0 && (
                          <div className={styles.projectTags}>
                            {project.skills.slice(0, 4).map((s) => (
                              <TechTag key={s.id} name={s.name} />
                            ))}
                          </div>
                        )}
                      </div>
                    </div>
                  )}
                </GlassCard>
              ))}
            </div>
          </div>
        </AnimatedSection>
      )}

      {/* ── Explore ── full-width band */}
      <AnimatedSection as="section" className={styles.exploreBand}>
        <div className={styles.bandInner}>
          <h2 className={styles.sectionTitle}>Explore</h2>
          <div className={styles.exploreGrid}>
            <GlassCard href="/projects">
              <div className={styles.exploreCard}>
                <span className={styles.exploreLabel}><TbFolder size={14} />Projects</span>
                <h3 className={styles.exploreTitle}>Backend design, API architecture, implementation decisions</h3>
                <span className={styles.exploreArrow}>→</span>
              </div>
            </GlassCard>
            <GlassCard href="/skills">
              <div className={styles.exploreCard}>
                <span className={styles.exploreLabel}><TbCpu size={14} />Skills</span>
                <h3 className={styles.exploreTitle}>Frameworks, platforms, and tooling choices</h3>
                <span className={styles.exploreArrow}>→</span>
              </div>
            </GlassCard>
            <GlassCard className={styles.exploreCardDisabled}>
              <div className={styles.exploreCard}>
                <span className={styles.exploreLabel}><TbBrain size={14} />Approach <span className={styles.comingSoon}>soon</span></span>
                <h3 className={styles.exploreTitle}>Architecture decisions, tradeoffs, platform-minded engineering</h3>
              </div>
            </GlassCard>
          </div>
        </div>
      </AnimatedSection>
    </div>
  );
}
