import { TbBraces, TbStack, TbBook, TbDatabase, TbTool, TbCloud, TbInfinity, TbBrain, TbCode, TbBrush, TbArrowsExchange, TbCpu, TbTestPipe, TbGitCommit } from 'react-icons/tb';
import type { IconType } from 'react-icons';

const CATEGORY_ICONS: Record<string, IconType> = {
  'language':       TbBraces,          // curly braces = syntax / code language
  'platform':       TbCpu,             // chip = runtime / platform layer
  'framework':      TbStack,           // layered structure
  'library':        TbBook,            // book
  'database':       TbDatabase,        // cylinder = database
  'orm':            TbArrowsExchange,  // bidirectional object ↔ row mapping
  'testing':        TbTestPipe,        // test tube = testing
  'tool':           TbTool,            // wrench/tool
  'styling':        TbBrush,           // brush = styling / CSS
  'source control': TbGitCommit,       // commit dot = version control
  'cloud service':  TbCloud,           // cloud
  'ci/cd':          TbInfinity,        // infinity loop = continuous integration/delivery
  'ai assistant':   TbBrain,           // brain
};

type Props = {
  category: string | null | undefined;
  size?: number;
  className?: string;
};

export default function CategoryIcon({ category, size = 20, className }: Props) {
  const Icon = CATEGORY_ICONS[(category ?? '').toLowerCase()] ?? TbCode;
  return <Icon size={size} className={className} aria-hidden="true" />;
}
