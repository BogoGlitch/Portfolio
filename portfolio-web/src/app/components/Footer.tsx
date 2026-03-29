import Image from "next/image";
import Link from "next/link";
import { FaGithub, FaLinkedinIn } from "react-icons/fa";
import { HiOutlineMail } from "react-icons/hi";
import styles from "./Footer.module.css";

const NAV_LINKS = [
  { href: "/",              label: "Home" },
  { href: "/projects",      label: "Projects" },
  { href: "/technologies",  label: "Technologies" },
];

export default function Footer() {
  const currentYear = new Date().getFullYear();

  return (
    <footer className={styles.footer}>
      <div className={styles.inner}>
        {/* Section A — Sitemap nav (left) */}
        <nav className={styles.sitemap} aria-label="Footer navigation">
          <ul className={styles.sitemapList}>
            {NAV_LINKS.map((link) => (
              <li key={link.href}>
                <Link href={link.href} className={styles.sitemapLink}>
                  {link.label}
                </Link>
              </li>
            ))}
          </ul>
        </nav>

        {/* Section B — Logo + name (center) */}
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

        {/* Section C — Social icons (right) */}
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
      </div>
    </footer>
  );
}
