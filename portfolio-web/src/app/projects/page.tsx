import { getProjects } from "@/lib/api";
import { Project } from "@/types/project";

export default async function ProjectsPage() {
  const projects: Project[] = await getProjects();
  return (
    <main>
      <h1>Projects</h1>

      {projects.map((project) => (
        <section key={project.id}>
          <h2>{project.name}</h2>
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
