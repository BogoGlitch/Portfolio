import { getProjects } from "@/lib/api";
import { Project } from "@/types/project";
import Link from "next/link";
import styles from "./page.module.css";

export default async function ProjectsPage() {
  const projects: Project[] = await getProjects();
  return (
    <main>
      <h1>Projects</h1>

      {projects.map((project) => (
        <section key={project.id} className={styles.section}>
          <h2>
            <Link href={`/projects/${project.slug}`}>{project.name}</Link>
          </h2>
          <p>{project.shortDescription}</p>
          <p>
            Technologies:{" "}
            {project.technologies.length > 0
              ? project.technologies.map((t) => t.name).join(", ")
              : "None currently associated with this project."}{" "}
          </p>
        </section>
      ))}
    </main>
  );
}
