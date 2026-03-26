import { ApiListResponse } from "@/types/api";
import { Project } from "@/types/project";
import { Technology } from "@/types/technology";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;

const ENDPOINTS = {
  projects: `${API_BASE_URL}/api/projects`,
  technologies: `${API_BASE_URL}/api/technologies`,
  auth: `${API_BASE_URL}/api/auth`,
} as const;

async function fetchJson<T>(url: string): Promise<T> {
  const res = await fetch(url);
  if (!res.ok) {
    const text = await res.text();
    throw new Error(`Failed to fetch projects: ${res.status} - ${text}`);
  }
  return res.json() as Promise<T>;
}

export async function checkAuth(): Promise<void> {
  const res = await fetch(`${ENDPOINTS.auth}/me`, { credentials: 'include' });
  if (!res.ok) throw new Error('Not authenticated');
}

export async function login(username: string, password: string): Promise<void> {
  const res = await fetch(`${ENDPOINTS.auth}/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({ username, password }),
  });
  if (!res.ok) throw new Error('Invalid credentials');
}

export async function logout(): Promise<void> {
  await fetch(`${ENDPOINTS.auth}/logout`, {
    method: 'POST',
    credentials: 'include',
  });
}

export async function getProjects(
  technologyIds?: string[],
): Promise<ApiListResponse<Project>> {
  const query = technologyIds?.length
    ? `?technologyIds=${technologyIds.join(",")}`
    : "";
  return fetchJson<ApiListResponse<Project>>(`${ENDPOINTS.projects}${query}`);
}

export async function getProjectBySlug(slug: string): Promise<Project> {
  return fetchJson<Project>(`${ENDPOINTS.projects}/${slug}`);
}

export async function getTechnologies(): Promise<ApiListResponse<Technology>> {
  return fetchJson<ApiListResponse<Technology>>(ENDPOINTS.technologies);
}

export async function getTechnologyBySlug(slug: string): Promise<Technology> {
  return fetchJson<Technology>(`${ENDPOINTS.technologies}/${slug}`);
}
