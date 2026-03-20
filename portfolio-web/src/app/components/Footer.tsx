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
        <div className={styles.columnLeft}>
          <h2 className={styles.heading}>Connect</h2>

          <div className={styles.iconLinks}>
            <a
              href="mailto:sean.bogolin@gmail.com"
              className={styles.iconLink}
              aria-label="Email Sean Bogolin"
            >
              <HiOutlineMail size={18} />
            </a>

            <a
              href="https://github.com/BogoGlitch"
              target="_blank"
              rel="noreferrer"
              className={styles.iconLink}
              aria-label="Sean Bogolin on GitHub"
            >
              <FaGithub size={18} />
            </a>

            <a
              href="https://www.linkedin.com/in/sean-bogolin/"
              target="_blank"
              rel="noreferrer"
              className={styles.iconLink}
              aria-label="Sean Bogolin on LinkedIn"
            >
              <FaLinkedinIn size={18} />
            </a>
          </div>
        </div>

        <div className={styles.columnCenter}>
          <Link href="/" className={styles.brand}>
            <Image
              src="/images/BogoLogo_GLITCH(b).png"
              alt="Sean Bogolin logo"
              width={36}
              height={36}
              className={styles.brandImage}
            />
            <span className={styles.brandText}>Sean Bogolin</span>
          </Link>

          <p className={styles.tagline}>
            Backend-first portfolio focused on architecture, APIs, and pragmatic software delivery.
          </p>
        </div>

        <div className={styles.columnRight}>
          <p className={styles.copyright}>© {currentYear} Sean Bogolin</p>
        </div>
      </div>
    </footer>
  );
}
