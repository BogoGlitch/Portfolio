'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { TbArrowLeft, TbEdit, TbPlus, TbTrash } from 'react-icons/tb';
import { useAuth } from '@/hooks/useAuth';
import {
  createProject,
  deleteProject,
  getProjects,
  getTechnologies,
  ProjectWriteDto,
  updateProject,
} from '@/lib/api';
import { Project, TechnologySummary } from '@/types/project';
import { Technology } from '@/types/technology';
import styles from './projects.module.css';

type FormState = {
  name: string;
  slug: string;
  shortDescription: string;
  fullDescription: string;
  repoUrl: string;
  liveUrl: string;
  imageUrl: string;
  isFeatured: boolean;
  displayOrder: string;
  technologyIds: number[];
};

const emptyForm: FormState = {
  name: '',
  slug: '',
  shortDescription: '',
  fullDescription: '',
  repoUrl: '',
  liveUrl: '',
  imageUrl: '',
  isFeatured: false,
  displayOrder: '0',
  technologyIds: [],
};

function slugify(name: string): string {
  return name.toLowerCase().replace(/\s+/g, '-').replace(/[^a-z0-9-]/g, '');
}

function formToDto(f: FormState): ProjectWriteDto {
  return {
    name: f.name,
    slug: f.slug,
    shortDescription: f.shortDescription,
    fullDescription: f.fullDescription,
    repoUrl: f.repoUrl || null,
    liveUrl: f.liveUrl || null,
    imageUrl: f.imageUrl || null,
    isFeatured: f.isFeatured,
    displayOrder: parseInt(f.displayOrder, 10) || 0,
    technologyIds: f.technologyIds,
  };
}

export default function ProjectsAdminPage() {
  const { isLoggedIn, isLoading } = useAuth();
  const router = useRouter();

  const [projects, setProjects] = useState<Project[]>([]);
  const [allTechs, setAllTechs] = useState<Technology[]>([]);
  const [modal, setModal] = useState<{ mode: 'create' | 'edit'; project?: Project } | null>(null);
  const [form, setForm] = useState<FormState>(emptyForm);
  const [slugManual, setSlugManual] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [confirmDelete, setConfirmDelete] = useState<number | null>(null);

  useEffect(() => {
    if (!isLoading && !isLoggedIn) router.push('/');
  }, [isLoading, isLoggedIn, router]);

  useEffect(() => {
    if (isLoggedIn) load();
  }, [isLoggedIn]);

  async function load() {
    const [projectsRes, techsRes] = await Promise.all([getProjects(), getTechnologies()]);
    setProjects(projectsRes as Project[]);
    setAllTechs(techsRes as Technology[]);
  }

  function openCreate() {
    setForm(emptyForm);
    setSlugManual(false);
    setError(null);
    setModal({ mode: 'create' });
  }

  function openEdit(project: Project) {
    setForm({
      name: project.name,
      slug: project.slug,
      shortDescription: project.shortDescription,
      fullDescription: project.fullDescription,
      repoUrl: project.repoUrl ?? '',
      liveUrl: project.liveUrl ?? '',
      imageUrl: project.imageUrl ?? '',
      isFeatured: project.isFeatured,
      displayOrder: String(project.displayOrder),
      technologyIds: project.technologies.map((t: TechnologySummary) => t.id),
    });
    setSlugManual(true);
    setError(null);
    setModal({ mode: 'edit', project });
  }

  function handleNameChange(value: string) {
    setForm(f => ({
      ...f,
      name: value,
      slug: slugManual ? f.slug : slugify(value),
    }));
  }

  function handleSlugChange(value: string) {
    setSlugManual(true);
    setForm(f => ({ ...f, slug: value }));
  }

  function toggleTech(id: number) {
    setForm(f => ({
      ...f,
      technologyIds: f.technologyIds.includes(id)
        ? f.technologyIds.filter(t => t !== id)
        : [...f.technologyIds, id],
    }));
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setSaving(true);
    setError(null);
    try {
      const dto = formToDto(form);
      if (modal?.mode === 'create') {
        await createProject(dto);
      } else if (modal?.project) {
        await updateProject(modal.project.id, dto);
      }
      setModal(null);
      await load();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Something went wrong');
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(id: number) {
    try {
      await deleteProject(id);
    } finally {
      setConfirmDelete(null);
      await load();
    }
  }

  if (isLoading || !isLoggedIn) return null;

  return (
    <main className={styles.page}>
      <div className={styles.pageHeader}>
        <div className={styles.pageHeaderLeft}>
          <Link href="/admin" className={styles.back}>
            <TbArrowLeft size={16} /> Admin
          </Link>
          <h1 className={styles.heading}>Projects</h1>
        </div>
        <button className={styles.addBtn} onClick={openCreate}>
          <TbPlus size={15} /> Add Project
        </button>
      </div>

      {projects.length === 0 ? (
        <p className={styles.empty}>No projects yet. Add one to get started.</p>
      ) : (
        <table className={styles.table}>
          <thead>
            <tr>
              <th>Name</th>
              <th>Slug</th>
              <th>Technologies</th>
              <th>Order</th>
              <th>Featured</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {projects.map(project => (
              <tr key={project.id}>
                <td className={styles.nameCell}>{project.name}</td>
                <td className={styles.slugCell}>{project.slug}</td>
                <td className={styles.techCell}>
                  {project.technologies.map(t => t.name).join(', ') || '—'}
                </td>
                <td>{project.displayOrder}</td>
                <td>{project.isFeatured ? '✓' : ''}</td>
                <td className={styles.actions}>
                  <button className={styles.iconBtn} onClick={() => openEdit(project)} title="Edit">
                    <TbEdit size={15} />
                  </button>
                  {confirmDelete === project.id ? (
                    <>
                      <button className={styles.confirmDeleteBtn} onClick={() => handleDelete(project.id)}>
                        Delete
                      </button>
                      <button className={styles.cancelBtn} onClick={() => setConfirmDelete(null)}>
                        Cancel
                      </button>
                    </>
                  ) : (
                    <button
                      className={`${styles.iconBtn} ${styles.dangerBtn}`}
                      onClick={() => setConfirmDelete(project.id)}
                      title="Delete"
                    >
                      <TbTrash size={15} />
                    </button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {modal && (
        <div className={styles.overlay} onClick={() => setModal(null)}>
          <div className={styles.modal} onClick={e => e.stopPropagation()}>
            <h2 className={styles.modalTitle}>
              {modal.mode === 'create' ? 'Add Project' : 'Edit Project'}
            </h2>
            <form onSubmit={handleSubmit} className={styles.form}>
              <div className={styles.twoCol}>
                <label className={styles.field}>
                  <span className={styles.label}>Name *</span>
                  <input
                    className={styles.input}
                    value={form.name}
                    onChange={e => handleNameChange(e.target.value)}
                    required
                  />
                </label>
                <label className={styles.field}>
                  <span className={styles.label}>Slug *</span>
                  <input
                    className={styles.input}
                    value={form.slug}
                    onChange={e => handleSlugChange(e.target.value)}
                    required
                  />
                </label>
              </div>

              <label className={styles.field}>
                <span className={styles.label}>Short Description *</span>
                <input
                  className={styles.input}
                  value={form.shortDescription}
                  onChange={e => setForm(f => ({ ...f, shortDescription: e.target.value }))}
                  required
                />
              </label>

              <label className={styles.field}>
                <span className={styles.label}>Full Description *</span>
                <textarea
                  className={styles.textarea}
                  value={form.fullDescription}
                  onChange={e => setForm(f => ({ ...f, fullDescription: e.target.value }))}
                  rows={4}
                  required
                />
              </label>

              <div className={styles.twoCol}>
                <label className={styles.field}>
                  <span className={styles.label}>Repo URL</span>
                  <input
                    className={styles.input}
                    value={form.repoUrl}
                    onChange={e => setForm(f => ({ ...f, repoUrl: e.target.value }))}
                    placeholder="https://github.com/…"
                  />
                </label>
                <label className={styles.field}>
                  <span className={styles.label}>Live URL</span>
                  <input
                    className={styles.input}
                    value={form.liveUrl}
                    onChange={e => setForm(f => ({ ...f, liveUrl: e.target.value }))}
                    placeholder="https://…"
                  />
                </label>
              </div>

              <label className={styles.field}>
                <span className={styles.label}>Image URL</span>
                <input
                  className={styles.input}
                  value={form.imageUrl}
                  onChange={e => setForm(f => ({ ...f, imageUrl: e.target.value }))}
                  placeholder="https://…"
                />
              </label>

              <div className={styles.twoCol}>
                <label className={styles.field}>
                  <span className={styles.label}>Display Order</span>
                  <input
                    className={styles.input}
                    type="number"
                    value={form.displayOrder}
                    onChange={e => setForm(f => ({ ...f, displayOrder: e.target.value }))}
                  />
                </label>
                <div />
              </div>

              <label className={styles.checkField}>
                <input
                  type="checkbox"
                  checked={form.isFeatured}
                  onChange={e => setForm(f => ({ ...f, isFeatured: e.target.checked }))}
                />
                <span>Featured</span>
              </label>

              {allTechs.length > 0 && (
                <div className={styles.field}>
                  <span className={styles.label}>Technologies</span>
                  <div className={styles.techList}>
                    {allTechs.map(tech => (
                      <label key={tech.id} className={styles.techItem}>
                        <input
                          type="checkbox"
                          checked={form.technologyIds.includes(tech.id)}
                          onChange={() => toggleTech(tech.id)}
                        />
                        <span>{tech.name}</span>
                      </label>
                    ))}
                  </div>
                </div>
              )}

              {error && <p className={styles.error}>{error}</p>}

              <div className={styles.formActions}>
                <button type="button" className={styles.cancelBtn} onClick={() => setModal(null)}>
                  Cancel
                </button>
                <button type="submit" className={styles.saveBtn} disabled={saving}>
                  {saving ? 'Saving…' : 'Save'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </main>
  );
}
