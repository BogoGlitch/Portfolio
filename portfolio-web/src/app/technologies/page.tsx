import Link from "next/link";
import { getTechnologies } from "@/lib/api";
import PageLayout from "../components/PageLayout";
import type { Metadata } from "next";

export const metadata: Metadata = {
  title: "Technologies",
  description: "Technologies, frameworks, and platform choices used across the portfolio.",
};

type TechnologiesPageProps = {
  searchParams?: Promise<{
    category?: string | string[];
  }>;
};

function parseCategory(value: string | string[] | undefined): string | undefined {
  const rawValue = Array.isArray(value) ? value[0] : value;
  const trimmedValue = rawValue?.trim();

  return trimmedValue ? trimmedValue : undefined;
}

export default async function TechnologiesPage({ searchParams }: TechnologiesPageProps) {
  const resolvedSearchParams = await searchParams;
  const selectedCategory = parseCategory(resolvedSearchParams?.category);

  const technologies = await getTechnologies();
  const filteredTechnologies = selectedCategory
    ? technologies.filter((technology) => technology.category === selectedCategory)
    : technologies;

  const categories = Array.from(
    new Set(technologies.map((t) => t.category).filter((c): c is string => Boolean(c))),
  ).sort();

  const categoryFilterFormKey = selectedCategory ?? "all";

  const categoryFilterSection = (
    <section>
      <form method="get" key={categoryFilterFormKey}>
        <fieldset>
          <legend>Filter by category</legend>

          <div className="filtersList">
            {categories.map((category) => (
              <label key={category}>
                <input
                  type="radio"
                  name="category"
                  value={category}
                  defaultChecked={selectedCategory === category}
                />
                {category}
              </label>
            ))}
          </div>

          <div className="ctas">
            <button type="submit">Apply Filter</button>
            <Link href="/technologies">Clear Filter</Link>
          </div>
        </fieldset>
      </form>
    </section>
  );

  return (
    <PageLayout
      undernav={<Link href="/">← Back to Home</Link>}
      title="Technologies"
      description="Explore the tools and platforms used across the portfolio, grouped by category."
    >
      {categoryFilterSection}
      <section>
        {filteredTechnologies.length === 0 ? (
          <p>
            {selectedCategory
              ? "No technologies matched the selected category filter."
              : "No technologies found."}
          </p>
        ) : (
          <ul>
            {filteredTechnologies.map((technology) => (
              <li key={technology.id}>
                <h2>
                  <Link href={`/technologies/${technology.slug}`}>{technology.name}</Link>
                </h2>

                {technology.category ? <p>Category: {technology.category}</p> : null}

                {technology.description ? <p>{technology.description}</p> : null}

                <p>
                  {technology.projects.length}{" "}
                  {technology.projects.length === 1 ? "project" : "projects"}
                </p>
              </li>
            ))}
          </ul>
        )}
      </section>
    </PageLayout>
  );
}
