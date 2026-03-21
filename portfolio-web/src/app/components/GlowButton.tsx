import Link from 'next/link';
import styles from './GlowButton.module.css';

type GlowButtonProps = {
  children: React.ReactNode;
  href?: string;
  onClick?: () => void;
  variant?: 'primary' | 'secondary' | 'ghost';
  external?: boolean;
  className?: string;
  type?: 'button' | 'submit';
};

export default function GlowButton({
  children,
  href,
  onClick,
  variant = 'primary',
  external = false,
  className = '',
  type = 'button',
}: GlowButtonProps) {
  const classes = [styles.btn, styles[variant], className].filter(Boolean).join(' ');

  if (href && external) {
    return (
      <a href={href} target="_blank" rel="noopener noreferrer" className={classes}>
        {children}
      </a>
    );
  }

  if (href) {
    return <Link href={href} className={classes}>{children}</Link>;
  }

  return (
    <button type={type} onClick={onClick} className={classes}>
      {children}
    </button>
  );
}
