import Link from 'next/link';
import styles from './GlassCard.module.css';

type GlassCardProps = {
  children: React.ReactNode;
  href?: string;
  className?: string;
  featured?: boolean;
  external?: boolean;
};

export default function GlassCard({ children, href, className = '', featured = false, external = false }: GlassCardProps) {
  const classes = [
    styles.card,
    featured ? styles.featured : '',
    className,
  ].filter(Boolean).join(' ');

  if (href && external) {
    return (
      <a href={href} target="_blank" rel="noopener noreferrer" className={classes}>
        {children}
      </a>
    );
  }

  if (href) {
    return (
      <Link href={href} className={classes}>
        {children}
      </Link>
    );
  }

  return <div className={classes}>{children}</div>;
}
