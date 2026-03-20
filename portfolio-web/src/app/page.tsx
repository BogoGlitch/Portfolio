import Image from "next/image";
import Link from "next/link";
import styles from "./page.module.css";

export default function HomePage() {
  return (
    <main className={styles.page}>
      <section className={styles.hero}>
        <div className={styles.heroContent}>
          <div className={styles.heroText}>
            <p className={styles.kicker}>Senior Software Engineer</p>
            <h1 className={styles.title}>Sean Bogolin</h1>
            <p className={styles.subtitle}>Backend-first portfolio platform</p>
            <p className={styles.description}>
              I design and build scalable, maintainable web platforms with a strong focus on API
              design, application architecture, and full-stack delivery.
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
          <ul className={styles.list}>
            <li>Backend architecture and API design</li>
            <li>Scalable and maintainable application structure</li>
            <li>Full-stack implementation with clear system boundaries</li>
          </ul>
        </section>

        <section className={styles.section}>
          <h2 className={styles.sectionTitle}>Explore</h2>

          <div className={styles.cardGrid}>
            <Link href="/projects" className={styles.card}>
              <article>
                <h3 className={styles.cardTitle}>Projects</h3>
                <p className={styles.cardText}>
                  Review portfolio work that highlights backend design, API architecture, and
                  end-to-end implementation decisions.
                </p>
              </article>
            </Link>

            <Link href="/technologies" className={styles.card}>
              <article>
                <h3 className={styles.cardTitle}>Technologies</h3>
                <p className={styles.cardText}>
                  Browse the technologies, frameworks, and platform choices used across the
                  portfolio.
                </p>
              </article>
            </Link>

            <article className={styles.card}>
              <h3 className={styles.cardTitle}>Approach</h3>
              <p className={styles.cardText}>
                Future space for architecture decisions, scalability, maintainability, security, and
                platform engineering thinking across the portfolio.
              </p>
            </article>
          </div>
        </section>
      </div>
    </main>
  );
}
