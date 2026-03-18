import Link from "next/link";
import { getTechnologyBySlug } from "@/lib/api";
import { notFound } from "next/navigation";

type TechnologyDetailPageProps = {
  params: Promise<{
    slug: string;
  }>;
};

export default async function TechnologyDetailPage({
  params,
}: TechnologyDetailPageProps) {
  const { slug } = await params;

  let technology;

  try {
    technology = await getTechnologyBySlug(slug);
  } catch {
    notFound();
  }

  return (
    <main>
      <p>
        <Link href="/technologies">← Back to Technologies</Link>
      </p>
      <section>
        <h1>{technology.name}</h1>

        {technology.description ? <p>{technology.description}</p> : null}
      </section>
      <section>
        {technology.category ? <p>Category: {technology.category}</p> : null}
      </section>
      <section>
        {technology.documentationUrl ? (
          <button>
            <a
              href={technology.documentationUrl}
              target="_blank"
              rel="noreferrer"
            >
              Documentation
            </a>
          </button>
        ) : null}
      </section>

      <section>
        <h3>Projects</h3>

        {technology.projects.length === 0 ? (
          <p>No projects are linked to this technology yet.</p>
        ) : (
          <ul>
            {technology.projects.map((project) => (
              <li key={project.id}>
                <Link href={`/projects/${project.slug}`}>{project.name}</Link>
              </li>
            ))}
          </ul>
        )}
      </section>
    </main>
  );
}
