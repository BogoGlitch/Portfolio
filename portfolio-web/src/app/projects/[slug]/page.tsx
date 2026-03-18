import { getProjectBySlug } from "@/lib/api";
import Link from "next/link";
import Image from "next/image";
import { notFound } from "next/navigation";
import styles from "../page.module.css";

type ProjectDetailPageProps = {
  params: Promise<{
    slug: string;
  }>;
};

export default async function ProjectDetailPage({
  params,
}: ProjectDetailPageProps) {
  const { slug } = await params;
  let project;

  try {
    project = await getProjectBySlug(slug);
  } catch {
    notFound();
  }

  return (
    <main>
      <Link href="/projects">← Back to Projects</Link>

      <section className={styles.section}>
        <h1>{project.name}</h1>
        <p>{project.shortDescription}</p>
      </section>

      {project.imageUrl && (
        <section className={styles.section}>
          <Image
            src={project.imageUrl}
            alt={project.name}
            width={600}
            height={400}
            className={styles.image}
            placeholder="blur"
            blurDataURL="data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw=="
          />
        </section>
      )}

      {project.fullDescription && (
        <section className={styles.section}>
          <h2>Description</h2>
          <p>{project.fullDescription}</p>
        </section>
      )}

      {(project.repoUrl || project.liveUrl) && (
        <section className={styles.section}>
          <h2>Links</h2>

          {project.repoUrl && (
            <div>
              <a
                href={project.repoUrl}
                target="_blank"
                rel="noopener noreferrer"
              >
                Repository
              </a>
            </div>
          )}

          {project.liveUrl && (
            <div>
              <a
                href={project.liveUrl}
                target="_blank"
                rel="noopener noreferrer"
              >
                Live Site
              </a>
            </div>
          )}
        </section>
      )}

      {project.technologies.length > 0 && (
        <section className={styles.section}>
          <h2>Technologies</h2>
          <ul>
            {project.technologies.map((tech) => (
              <li key={tech.id}>{tech.name}</li>
            ))}
          </ul>
        </section>
      )}
    </main>
  );
}
