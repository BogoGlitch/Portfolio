import Image from "next/image";
import Link from "next/link";
import styles from "./page.module.css";
import { Metadata } from "next";

export const metadata: Metadata = {
  title: "Home | Sean Bogolin",
  description:
    "Backend-first portfolio platform showcasing projects, technologies, and pragmatic software engineering decisions.",
};

export default function HomePage() {
  return (
    <main className={styles.page}>
      <section className={styles.hero}>
        <div className={styles.heroContent}>
          <div className={styles.heroText}>
            <p className={styles.kicker}>Senior Software Engineer</p>
            <h1 className={styles.title}>Sean Bogolin</h1>
            <p className={styles.subtitle}>Backend-first engineering showcase</p>
            <p className={styles.description}>
              I design and build scalable, maintainable web platforms with a strong focus on backend
              architecture, API design, and end-to-end delivery.
            </p>

            <div className={styles.heroActions}>
              <Link href="/projects" className={styles.primaryAction}>
                View Projects
              </Link>
              <Link href="/technologies" className={styles.secondaryAction}>
                Browse Technologies
              </Link>
            </div>
          </div>

          <div className={styles.heroMedia}>
            <Image
              src="/images/headshot-placeholder.jpg"
              alt="Portrait of Sean Bogolin"
              width={420}
              height={420}
              className={styles.heroImage}
              priority
            />
          </div>
        </div>
      </section>

      <div className={styles.content}>
        <section className={styles.section}>
          <h2 className={styles.sectionTitle}>What this portfolio demonstrates</h2>

          <div className={styles.highlightPanel}>
            <p className={styles.highlightIntro}>
              This platform is designed to show more than finished screens. It highlights how I
              think through backend architecture, API contracts, data modeling, and pragmatic
              delivery.
            </p>

            <div className={styles.highlightGrid}>
              <article className={styles.highlightItem}>
                <h3 className={styles.highlightTitle}>Backend-first design</h3>
                <p className={styles.highlightText}>
                  Clear service boundaries, DTO-driven APIs, and data access shaped for
                  maintainability.
                </p>
              </article>

              <article className={styles.highlightItem}>
                <h3 className={styles.highlightTitle}>Pragmatic architecture</h3>
                <p className={styles.highlightText}>
                  Decisions grounded in tradeoffs, scalability, readability, and delivery maturity.
                </p>
              </article>

              <article className={styles.highlightItem}>
                <h3 className={styles.highlightTitle}>End-to-end ownership</h3>
                <p className={styles.highlightText}>
                  From database and API design through frontend integration and deployment
                  readiness.
                </p>
              </article>

              <article className={styles.highlightItem}>
                <h3 className={styles.highlightTitle}>Enterprise mindset</h3>
                <p className={styles.highlightText}>
                  Structured patterns, explicit configuration, and systems built for long-term
                  evolution.
                </p>
              </article>
            </div>
          </div>
        </section>

        <section className={styles.section}>
          <h2 className={styles.sectionTitle}>Explore</h2>

          <div className={styles.cardGrid}>
            <Link href="/projects" className={styles.card}>
              <article>
                <div className={styles.cardMedia}>
                  <Image
                    src="https://picsum.photos/seed/projects-card/800/450"
                    alt=""
                    width={800}
                    height={450}
                    className={styles.cardImage}
                  />
                </div>
                <h3 className={styles.cardTitle}>Projects</h3>
                <p className={styles.cardText}>
                  Review portfolio work that highlights backend design, API architecture, and
                  end-to-end implementation decisions.
                </p>
              </article>
            </Link>

            <Link href="/technologies" className={styles.card}>
              <article>
                <div className={styles.cardMedia}>
                  <Image
                    src="https://picsum.photos/seed/technologies-card/800/450"
                    alt=""
                    width={800}
                    height={450}
                    className={styles.cardImage}
                  />
                </div>
                <h3 className={styles.cardTitle}>Technologies</h3>
                <p className={styles.cardText}>
                  Browse the technologies, frameworks, and platform choices used across the
                  portfolio.
                </p>
              </article>
            </Link>

            <article className={`${styles.card} ${styles.cardDisabled}`}>
              <div className={styles.cardMedia}>
                <Image
                  src="https://picsum.photos/seed/approach-card/800/450"
                  alt=""
                  width={800}
                  height={450}
                  className={styles.cardImage}
                />
              </div>
              <h3 className={styles.cardTitle}>Approach</h3>
              <p className={styles.cardText}>
                A future section focused on architecture decisions, tradeoffs, maintainability,
                scalability, and platform-minded engineering.
              </p>
            </article>
          </div>
        </section>
      </div>
    </main>
  );
}
