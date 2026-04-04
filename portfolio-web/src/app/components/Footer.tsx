import Image from "next/image";
import Link from "next/link";
import { FaGithub, FaLinkedinIn } from "react-icons/fa";
import { HiOutlineMail } from "react-icons/hi";
import styles from "./Footer.module.css";

const NAV_LINKS = [
  { href: "/",              label: "Home" },
  { href: "/projects",      label: "Projects" },
  { href: "/skills",         label: "Skills" },
  { href: null,             label: "Approach",     disabled: true },
];

export default function Footer() {
  const currentYear = new Date().getFullYear();

  return (
    <footer className={styles.footer}>
      <div className={styles.inner}>
        {/* Nav links (left on desktop, top on mobile) */}
        <nav className={styles.sitemap} aria-label="Footer navigation">
          <ul className={styles.sitemapList}>
            {NAV_LINKS.map((link) => (
              <li key={link.label}>
                {link.disabled ? (
                  <span className={`${styles.sitemapLink} ${styles.sitemapLinkDisabled}`}>
                    {link.label}
                  </span>
                ) : (
                  <Link href={link.href!} className={styles.sitemapLink}>
                    {link.label}
                  </Link>
                )}
              </li>
            ))}
          </ul>
        </nav>

        <hr className={styles.divider} />

        {/* Social icons (right on desktop, middle on mobile) */}
        <div className={styles.connect}>
          <div className={styles.iconLinks}>
            <a href="mailto:sean.bogolin@gmail.com" className={styles.iconLink} aria-label="Email">
              <HiOutlineMail size={17} />
            </a>
            <a href="https://github.com/BogoGlitch" target="_blank" rel="noreferrer" className={styles.iconLink} aria-label="GitHub">
              <FaGithub size={17} />
            </a>
            <a href="https://www.linkedin.com/in/sean-bogolin/" target="_blank" rel="noreferrer" className={styles.iconLink} aria-label="LinkedIn">
              <FaLinkedinIn size={17} />
            </a>
          </div>
        </div>

        <hr className={styles.divider} />

        {/* Logo + copyright (center on desktop, bottom on mobile) */}
        <div className={styles.brand}>
          <Link href="/" className={styles.brandLink}>
            <Image
              src="/images/BogoLogo_GLITCH(b).png"
              alt="Sean Bogolin logo"
              width={36}
              height={36}
              className={styles.brandImage}
            />
            <span className={styles.brandText}>Sean Bogolin</span>
          </Link>
          <p className={styles.copyright}>&copy; {currentYear} Sean Bogolin</p>
        </div>
      </div>
    </footer>
  );
}
