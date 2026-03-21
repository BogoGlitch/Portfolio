import {
  SiDotnet,
  SiSharp,
  SiNextdotjs,
  SiReact,
  SiTypescript,
  SiJavascript,
  SiPostgresql,
  SiMysql,
  SiMongodb,
  SiRedis,
  SiDocker,
  SiGithub,
  SiTailwindcss,
  SiNodedotjs,
  SiPython,
  SiGo,
  SiRust,
  SiKubernetes,
  SiTerraform,
  SiGraphql,
  SiOpenai,
  SiHtml5,
  SiCss,
  SiSass,
} from 'react-icons/si';
import { TbDatabase, TbCloud, TbCode } from 'react-icons/tb';
import type { IconType } from 'react-icons';

const ICON_MAP: Record<string, IconType> = {
  // .NET ecosystem
  'dotnet':           SiDotnet,
  'dot-net':          SiDotnet,
  'aspnet':           SiDotnet,
  'asp-net':          SiDotnet,
  'asp-net-core':     SiDotnet,
  'csharp':           SiSharp,
  'c-sharp':          SiSharp,
  'entity-framework': TbDatabase,

  // Frontend
  'nextjs':           SiNextdotjs,
  'next-js':          SiNextdotjs,
  'react':            SiReact,
  'typescript':       SiTypescript,
  'javascript':       SiJavascript,
  'html':             SiHtml5,
  'html5':            SiHtml5,
  'css':              SiCss,
  'css3':             SiCss,
  'sass':             SiSass,
  'tailwindcss':      SiTailwindcss,
  'tailwind':         SiTailwindcss,

  // Databases
  'sql-server':       TbDatabase,
  'mssql':            TbDatabase,
  'postgresql':       SiPostgresql,
  'postgres':         SiPostgresql,
  'mysql':            SiMysql,
  'mongodb':          SiMongodb,
  'redis':            SiRedis,

  // Cloud & infra
  'azure':            TbCloud,
  'docker':           SiDocker,
  'kubernetes':       SiKubernetes,
  'terraform':        SiTerraform,

  // Other languages / runtimes
  'nodejs':           SiNodedotjs,
  'node-js':          SiNodedotjs,
  'python':           SiPython,
  'go':               SiGo,
  'golang':           SiGo,
  'rust':             SiRust,

  // APIs / AI
  'graphql':          SiGraphql,
  'openai':           SiOpenai,

  // VCS
  'github':           SiGithub,
};

type TechIconProps = {
  slug: string;
  size?: number;
  className?: string;
};

export default function TechIcon({ slug, size = 20, className }: TechIconProps) {
  const normalised = slug.toLowerCase().trim();
  const Icon = ICON_MAP[normalised];

  if (!Icon) {
    const FallbackIcon = TbCode as IconType;
    return <FallbackIcon size={size} className={className} aria-hidden="true" />;
  }

  return <Icon size={size} className={className} aria-hidden="true" />;
}

export function hasTechIcon(slug: string): boolean {
  return slug.toLowerCase().trim() in ICON_MAP;
}
