export type TechnologySummary = {
  id: number;
  name: string;
  slug: string;
};

export type Project = {
  id: number;
  name: string;
  slug: string;
  shortDescription: string | null;
  technologies: TechnologySummary[];
};
