import Link from 'next/link';
import styles from './TechTag.module.css';

type TechTagProps = {
  name: string;
  slug?: string;
};

export default function TechTag({ name, slug }: TechTagProps) {
  if (slug) {
    return (
      <Link href={`/skills/${slug}`} className={styles.tag}>
        {name}
      </Link>
    );
  }

  return <span className={styles.tag}>{name}</span>;
}
