'use client';

import { TbBolt, TbFlame, TbMoonStars, TbPrism } from 'react-icons/tb';
import { useTheme, type Theme } from '@/hooks/useTheme';
import styles from './ThemeToggle.module.css';

const ICONS: Record<Theme, React.ElementType> = {
  cosmos: TbMoonStars,
  glitch: TbBolt,
  ember:  TbFlame,
  prism:  TbPrism,
};

const NEXT_LABEL: Record<Theme, string> = {
  cosmos: 'Switch to Glitch',
  glitch: 'Switch to Ember',
  ember:  'Switch to Prism',
  prism:  'Switch to Cosmos',
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
