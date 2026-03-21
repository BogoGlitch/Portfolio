'use client';

import { TbBolt, TbFlame, TbMoonStars } from 'react-icons/tb';
import { useTheme, type Theme } from '@/hooks/useTheme';
import styles from './ThemeToggle.module.css';

const ICONS: Record<Theme, React.ElementType> = {
  glitch: TbBolt,
  ember:  TbFlame,
  cosmos: TbMoonStars,
};

const NEXT_LABEL: Record<Theme, string> = {
  glitch: 'Switch to Ember',
  ember:  'Switch to Cosmos',
  cosmos: 'Switch to Glitch',
};

export default function ThemeToggle() {
  const { theme, cycleTheme } = useTheme();
  const Icon = ICONS[theme];

  return (
    <button
      className={styles.toggle}
      onClick={cycleTheme}
      aria-label={NEXT_LABEL[theme]}
      title={NEXT_LABEL[theme]}
    >
      <Icon size={17} className={styles.icon} />
      <span className={styles.label}>{theme}</span>
    </button>
  );
}
