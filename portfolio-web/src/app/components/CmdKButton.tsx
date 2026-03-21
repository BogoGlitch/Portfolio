'use client';

import styles from './Header.module.css';

export default function CmdKButton() {
  return (
    <button
      className={styles.kbdHint}
      aria-label="Open command palette"
      onClick={() => window.dispatchEvent(new CustomEvent('open-command-palette'))}
    >
      <span>⌘K</span>
    </button>
  );
}
