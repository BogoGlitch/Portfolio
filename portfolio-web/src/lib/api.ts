import { ApiListResponse } from "@/types/api";
import { Project } from "@/types/project";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;

const ENDPOINTS = {
  projects: `${API_BASE_URL}/api/projects`,
} as const;

async function fetchJson<T>(url: string): Promise<T> {
  const res = await fetch(url);
  if (!res.ok) {
    const text = await res.text();
    throw new Error(`Failed to fetch projects: ${res.status} - ${text}`);
  }
  return res.json() as Promise<T>;
}

export async function getProjects(): Promise<ApiListResponse<Project>> {
  return fetchJson<Project[]>(ENDPOINTS.projects);
}
