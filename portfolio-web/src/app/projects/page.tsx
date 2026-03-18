import { getProjects, getTechnologies } from "@/lib/api";
import Link from "next/link";
import styles from "./page.module.css";

type ProjectsPageProps = {
  searchParams?: Promise<{
    technologyIds?: string | string[];
  }>;
};

function parseTechnologyIds(value: string | string[] | undefined): string[] {
  const rawValues = Array.isArray(value) ? value : value ? [value] : [];

  return rawValues
    .flatMap((item) => item.split(","))
    .map((item) => item.trim())
    .filter(Boolean);
}

export default async function ProjectsPage({
  searchParams,
}: ProjectsPageProps) {
  const resolvedSearchParams = await searchParams;

  const selectedTechnologyIds = parseTechnologyIds(
    resolvedSearchParams?.technologyIds,
  );

  const [projects, technologies] = await Promise.all([
    getProjects(selectedTechnologyIds),
    getTechnologies(),
  ]);

  const technologyFilterFormKey = selectedTechnologyIds.join(",");

  const technologyFiltersSection = (
    <section className={styles.section}>
      <form method="get" key={technologyFilterFormKey}>
        <fieldset>
          <legend>Filter by technology</legend>

          <div className={styles.filter}>
            {technologies.map((technology) => (
              <label key={technology.id} className={styles.label}>
                <input
                  type="checkbox"
                  name="technologyIds"
                  value={technology.id}
                  defaultChecked={selectedTechnologyIds.includes(
                    String(technology.id),
                  )}
                />
                {technology.name}
              </label>
            ))}
          </div>
          <div className={styles.ctas}>
            <button className={styles.submit} type="submit">
              Apply Filters
            </button>
            <Link href="/projects">Clear Filters</Link>
          </div>
        </fieldset>
      </form>
    </section>
  );

  return (
    <main>
      <h1>Projects</h1>
      {technologyFiltersSection}
      {projects.length === 0 ? (
        <section className={styles.section}>
          <p>No projects matched the selected technology filters.</p>
        </section>
      ) : (
        projects.map((project) => (
          <section key={project.id} className={styles.section}>
            <h2>
              <Link href={`/projects/${project.slug}`}>{project.name}</Link>
            </h2>
            <p>{project.shortDescription}</p>
            <p>
              Technologies:{" "}
              {project.technologies.length > 0
                ? project.technologies.map((t) => t.name).join(", ")
                : "None currently associated with this project."}
            </p>
          </section>
        ))
      )}
    </main>
  );
}
