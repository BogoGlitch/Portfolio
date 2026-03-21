import Image from "next/image";
import Link from "next/link";
import { FaGithub, FaLinkedinIn } from "react-icons/fa";
import { HiOutlineMail } from "react-icons/hi";
import styles from "./Footer.module.css";

export default function Footer() {
  const currentYear = new Date().getFullYear();

  return (
    <footer className={styles.footer}>
      <div className={styles.inner}>
        <div className={styles.left}>
          <Link href="/" className={styles.brand}>
            <Image
              src="/images/BogoLogo_GLITCH(b).png"
              alt="Sean Bogolin logo"
              width={32}
              height={32}
              className={styles.brandImage}
            />
            <span className={styles.brandText}>Sean Bogolin</span>
          </Link>
          <p className={styles.tagline}>
            Backend-first engineering — architecture, APIs, and pragmatic delivery.
          </p>
        </div>

        <div className={styles.right}>
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
          <p className={styles.copyright}>© {currentYear} Sean Bogolin</p>
        </div>
      </div>
    </footer>
  );
}
