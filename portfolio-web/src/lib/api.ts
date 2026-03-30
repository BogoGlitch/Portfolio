import { cache } from "react";
import { ApiListResponse } from "@/types/api";
import { Project } from "@/types/project";
import { Technology } from "@/types/technology";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;

const ENDPOINTS = {
  projects: `${API_BASE_URL}/api/projects`,
  technologies: `${API_BASE_URL}/api/technologies`,
  auth: `${API_BASE_URL}/api/auth`,
} as const;

async function fetchWithTimeout(url: string, timeoutMs: number): Promise<Response> {
  const controller = new AbortController();
  const id = setTimeout(() => controller.abort(), timeoutMs);
  try {
    return await fetch(url, { signal: controller.signal });
  } finally {
    clearTimeout(id);
  }
}

async function fetchJson<T>(url: string): Promise<T> {
  const maxAttempts = 2;
  const timeoutMs = 15_000;

  for (let attempt = 1; attempt <= maxAttempts; attempt++) {
    try {
      const res = await fetchWithTimeout(url, timeoutMs);
      if (!res.ok) {
        const text = await res.text();
        throw new Error(`API error: ${res.status} - ${text}`);
      }
      return res.json() as Promise<T>;
    } catch (err) {
      if (attempt === maxAttempts) throw err;
      // Brief pause before retry — gives cold-starting API a moment
      await new Promise((r) => setTimeout(r, 2_000));
    }
  }

  // unreachable, but satisfies TypeScript
  throw new Error("fetchJson: exhausted retries");
}

async function mutateJson<T>(url: string, method: string, body?: unknown): Promise<T> {
  const res = await fetch(url, {
    method,
    credentials: 'include',
    headers: body ? { 'Content-Type': 'application/json' } : {},
    body: body ? JSON.stringify(body) : undefined,
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(`${res.status} - ${text}`);
  }
  if (res.status === 204) return undefined as T;
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

export async function getProjects(options?: {
  technologyIds?: string[];
  discipline?: string;
}): Promise<ApiListResponse<Project>> {
  const params = new URLSearchParams();
  if (options?.technologyIds?.length) params.set("technologyIds", options.technologyIds.join(","));
  if (options?.discipline) params.set("discipline", options.discipline);
  const query = params.size > 0 ? `?${params.toString()}` : "";
  return fetchJson<ApiListResponse<Project>>(`${ENDPOINTS.projects}${query}`);
}

export const getProjectBySlug = cache(async (slug: string): Promise<Project> => {
  return fetchJson<Project>(`${ENDPOINTS.projects}/${slug}`);
});

export async function getTechnologies(): Promise<ApiListResponse<Technology>> {
  return fetchJson<ApiListResponse<Technology>>(ENDPOINTS.technologies);
}

export const getTechnologyBySlug = cache(async (slug: string): Promise<Technology> => {
  return fetchJson<Technology>(`${ENDPOINTS.technologies}/${slug}`);
});

export type TechnologyWriteDto = {
  name: string;
  slug: string;
  description: string;
  category: string;
  discipline: string;
  logoUrl: string | null;
  documentationUrl: string | null;
  isFeatured: boolean;
  displayOrder: number;
};

export async function createTechnology(dto: TechnologyWriteDto): Promise<Technology> {
  return mutateJson<Technology>(ENDPOINTS.technologies, 'POST', dto);
}

export async function updateTechnology(id: number, dto: TechnologyWriteDto): Promise<Technology> {
  return mutateJson<Technology>(`${ENDPOINTS.technologies}/${id}`, 'PUT', dto);
}

export async function deleteTechnology(id: number): Promise<void> {
  return mutateJson<void>(`${ENDPOINTS.technologies}/${id}`, 'DELETE');
}

export type ProjectWriteDto = {
  name: string;
  slug: string;
  shortDescription: string;
  fullDescription: string;
  repoUrl: string | null;
  liveUrl: string | null;
  imageUrl: string | null;
  isFeatured: boolean;
  displayOrder: number;
  technologyIds: number[];
};

export async function createProject(dto: ProjectWriteDto): Promise<Project> {
  return mutateJson<Project>(ENDPOINTS.projects, 'POST', dto);
}

export async function updateProject(id: number, dto: ProjectWriteDto): Promise<Project> {
  return mutateJson<Project>(`${ENDPOINTS.projects}/${id}`, 'PUT', dto);
}

export async function deleteProject(id: number): Promise<void> {
  return mutateJson<void>(`${ENDPOINTS.projects}/${id}`, 'DELETE');
}
