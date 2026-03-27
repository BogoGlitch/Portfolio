import { TbCode, TbServer, TbDatabase, TbCloud, TbGitBranch, TbBrain, TbLayoutGrid } from 'react-icons/tb';
import type { IconType } from 'react-icons';

const DISCIPLINE_ICONS: Record<string, IconType> = {
  'frontend':  TbCode,
  'backend':   TbServer,
  'database':  TbDatabase,
  'cloud':     TbCloud,
  'devops':    TbGitBranch,
  'ai':        TbBrain,
};

type Props = {
  discipline: string;
  size?: number;
  className?: string;
};

export default function DisciplineIcon({ discipline, size = 16, className }: Props) {
  const Icon = DISCIPLINE_ICONS[discipline.toLowerCase()] ?? TbLayoutGrid;
  return <Icon size={size} className={className} aria-hidden="true" />;
}
