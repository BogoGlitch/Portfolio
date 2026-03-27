import { TbBraces, TbStack, TbBook, TbDatabase, TbTool, TbCloud, TbInfinity, TbBrain, TbCode, TbBrush, TbArrowsExchange } from 'react-icons/tb';
import type { IconType } from 'react-icons';

const CATEGORY_ICONS: Record<string, IconType> = {
  'language':      TbBraces,          // curly braces = syntax / code language
  'framework':     TbStack,           // layered structure
  'library':       TbBook,            // book
  'orm':           TbArrowsExchange,  // bidirectional object ↔ row mapping
  'tool':          TbTool,            // wrench/tool
  'cloud service': TbCloud,           // cloud
  'ci/cd':         TbInfinity,        // infinity loop = continuous integration/delivery
  'ai assistant':  TbBrain,           // brain
  'database':      TbDatabase,        // cylinder = database
  'styling':       TbBrush,           // brush = styling / CSS
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
