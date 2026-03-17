export type TechnologySummary = {
  id: number;
  name: string;
  slug: string;
};

export type Project = {
  id: number;
  name: string;
  slug: string;
  shortDescription: string;
  fullDescription: string;
  repoUrl: string | null;
  liveUrl: string | null;
  imageUrl: string | null;
  isFeatured: boolean;
  displayOrder: number;
  technologies: TechnologySummary[];
};
