'use client';

import Image from "next/image";
import Link from "next/link";
import ThemeToggle from "./ThemeToggle";
import AuthButton from "./AuthButton";
import MobileNav from "./MobileNav";
import CmdKButton from "./CmdKButton";
import { useScrollDirection } from "@/hooks/useScrollDirection";
import styles from "./Header.module.css";

export default function Header() {
  const hidden = useScrollDirection();

  return (
    <header className={`${styles.header}${hidden ? ` ${styles.headerHidden}` : ''}`}>
      <div className={styles.inner}>
        <Link href="/" className={styles.brand}>
          <Image
            src="/images/BogoLogo_GLITCH(b).png"
            alt="Sean Bogolin logo"
            width={36}
            height={36}
            className={styles.brandImage}
            priority
          />
          <span className={styles.brandText}>Sean Bogolin</span>
        </Link>

        <nav className={styles.nav} aria-label="Main navigation">
          <Link href="/projects" className={styles.navLink}>Projects</Link>
          <Link href="/technologies" className={styles.navLink}>Technologies</Link>
          <span className={`${styles.navLink} ${styles.navLinkDisabled}`} aria-disabled="true">
            Approach
          </span>
        </nav>

        <div className={styles.actions}>
          <CmdKButton />
          <ThemeToggle />
          <AuthButton />
          <MobileNav />
        </div>
      </div>
    </header>
  );
}
