import Link from "next/link";

export default function TechnologyNotFound() {
  return (
    <div>
      <h1>Technology not found</h1>
      <p>The technology you are looking for does not exist.</p>
      <Link href="/technologies">Back to Technologies</Link>
    </div>
  );
}
