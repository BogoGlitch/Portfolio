export type ProjectSummary = {
  id: number;
  name: string;
  slug: string;
};

export type Skill = {
  id: number;
  name: string;
  slug: string;
  description: string | null;
  category: string | null;
  discipline: string;
  logoUrl: string | null;
  documentationUrl: string | null;
  isFeatured: boolean;
  displayOrder: number;
  projects: ProjectSummary[];
};
