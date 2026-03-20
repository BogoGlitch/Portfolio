import Image from "next/image";
import Link from "next/link";
import styles from "./Header.module.css";

export default function Header() {
  return (
    <header className={styles.header}>
      <div className={styles.inner}>
        <Link href="/" className={styles.brand}>
          <Image
            src="/images/BogoLogo_GLITCH(b).png"
            alt="Sean Bogolin logo"
            width={40}
            height={40}
            className={styles.brandImage}
            priority
          />
          <span className={styles.brandText}>Sean Bogolin</span>
        </Link>

        <nav className={styles.nav} aria-label="Main navigation">
          <Link href="/projects" className={styles.navLink}>
            Projects
          </Link>
          <Link href="/technologies" className={styles.navLink}>
            Technologies
          </Link>
          <span className={`${styles.navLink} ${styles.navLinkDisabled}`} aria-disabled="true">
            Approach
          </span>
        </nav>
      </div>
    </header>
  );
}
