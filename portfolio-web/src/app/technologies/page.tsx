import Link from "next/link";
import { getTechnologies } from "@/lib/api";

export default async function TechnologiesPage() {
  const technologies = await getTechnologies();

  return (
    <main>
      <h1>Technologies</h1>

      <section>
        {technologies.length === 0 ? (
          <p>No technologies found.</p>
        ) : (
          <ul>
            {technologies.map((technology) => (
              <li key={technology.id}>
                <h2>
                  <Link href={`/technologies/${technology.slug}`}>
                    {technology.name}
                  </Link>
                </h2>

                {technology.category ? (
                  <p>Category: {technology.category}</p>
                ) : null}

                {technology.description ? (
                  <p>{technology.description}</p>
                ) : null}

                <p>
                  {technology.projects.length}{" "}
                  {technology.projects.length === 1 ? "project" : "projects"}
                </p>
              </li>
            ))}
          </ul>
        )}
      </section>
    </main>
  );
}
