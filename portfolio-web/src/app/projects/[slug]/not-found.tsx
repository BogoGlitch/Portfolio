import Link from "next/link";

export default function ProjectNotFound() {
  return (
    <div>
      <h1>Project not found</h1>
      <p>The project you are looking for does not exist.</p>
      <Link href="/projects">Back to Projects</Link>
    </div>
  );
}
