'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { TbArrowLeft, TbEdit, TbPlus, TbTrash } from 'react-icons/tb';
import { useAuth } from '@/hooks/useAuth';
import {
  createTechnology,
  deleteTechnology,
  getTechnologies,
  TechnologyWriteDto,
  updateTechnology,
} from '@/lib/api';
import { Technology } from '@/types/technology';
import styles from './technologies.module.css';

const DISCIPLINES = ['Frontend', 'Backend', 'Database', 'Cloud', 'DevOps', 'AI'] as const;
type Discipline = typeof DISCIPLINES[number];

type FormState = {
  name: string;
  slug: string;
  description: string;
  category: string;
  discipline: Discipline | '';
  logoUrl: string;
  documentationUrl: string;
  isFeatured: boolean;
  displayOrder: string;
};

const emptyForm: FormState = {
  name: '',
  slug: '',
  description: '',
  category: '',
  discipline: '',
  logoUrl: '',
  documentationUrl: '',
  isFeatured: false,
  displayOrder: '0',
};

function slugify(name: string): string {
  return name.toLowerCase().replace(/\s+/g, '-').replace(/[^a-z0-9-]/g, '');
}

function formToDto(f: FormState): TechnologyWriteDto {
  return {
    name: f.name,
    slug: f.slug,
    description: f.description,
    category: f.category,
    discipline: f.discipline as Discipline,
    logoUrl: f.logoUrl || null,
    documentationUrl: f.documentationUrl || null,
    isFeatured: f.isFeatured,
    displayOrder: parseInt(f.displayOrder, 10) || 0,
  };
}

export default function TechnologiesAdminPage() {
  const { isLoggedIn, isLoading } = useAuth();
  const router = useRouter();

  const [technologies, setTechnologies] = useState<Technology[]>([]);
  const [modal, setModal] = useState<{ mode: 'create' | 'edit'; tech?: Technology } | null>(null);
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
    const items = await getTechnologies();
    setTechnologies(items as Technology[]);
  }

  function openCreate() {
    setForm(emptyForm);
    setSlugManual(false);
    setError(null);
    setModal({ mode: 'create' });
  }

  function openEdit(tech: Technology) {
    setForm({
      name: tech.name,
      slug: tech.slug,
      description: tech.description ?? '',
      category: tech.category ?? '',
      discipline: (tech.discipline as Discipline) ?? '',
      logoUrl: tech.logoUrl ?? '',
      documentationUrl: tech.documentationUrl ?? '',
      isFeatured: tech.isFeatured,
      displayOrder: String(tech.displayOrder),
    });
    setSlugManual(true);
    setError(null);
    setModal({ mode: 'edit', tech });
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

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setSaving(true);
    setError(null);
    try {
      const dto = formToDto(form);
      if (modal?.mode === 'create') {
        await createTechnology(dto);
      } else if (modal?.tech) {
        await updateTechnology(modal.tech.id, dto);
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
      await deleteTechnology(id);
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
          <h1 className={styles.heading}>Technologies</h1>
        </div>
        <button className={styles.addBtn} onClick={openCreate}>
          <TbPlus size={15} /> Add Technology
        </button>
      </div>

      {technologies.length === 0 ? (
        <p className={styles.empty}>No technologies yet. Add one to get started.</p>
      ) : (
        <table className={styles.table}>
          <thead>
            <tr>
              <th>Name</th>
              <th>Category</th>
              <th>Discipline</th>
              <th>Slug</th>
              <th>Order</th>
              <th>Featured</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {technologies.map(tech => (
              <tr key={tech.id}>
                <td className={styles.nameCell}>{tech.name}</td>
                <td>{tech.category ?? '—'}</td>
                <td>{tech.discipline}</td>
                <td className={styles.slugCell}>{tech.slug}</td>
                <td>{tech.displayOrder}</td>
                <td>{tech.isFeatured ? '✓' : ''}</td>
                <td className={styles.actions}>
                  <button className={styles.iconBtn} onClick={() => openEdit(tech)} title="Edit">
                    <TbEdit size={15} />
                  </button>
                  {confirmDelete === tech.id ? (
                    <>
                      <button className={styles.confirmDeleteBtn} onClick={() => handleDelete(tech.id)}>
                        Delete
                      </button>
                      <button className={styles.cancelBtn} onClick={() => setConfirmDelete(null)}>
                        Cancel
                      </button>
                    </>
                  ) : (
                    <button
                      className={`${styles.iconBtn} ${styles.dangerBtn}`}
                      onClick={() => setConfirmDelete(tech.id)}
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
              {modal.mode === 'create' ? 'Add Technology' : 'Edit Technology'}
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
                <span className={styles.label}>Description *</span>
                <textarea
                  className={styles.textarea}
                  value={form.description}
                  onChange={e => setForm(f => ({ ...f, description: e.target.value }))}
                  rows={3}
                  required
                />
              </label>

              <div className={styles.twoCol}>
                <label className={styles.field}>
                  <span className={styles.label}>Category *</span>
                  <input
                    className={styles.input}
                    value={form.category}
                    onChange={e => setForm(f => ({ ...f, category: e.target.value }))}
                    required
                  />
                </label>
                <label className={styles.field}>
                  <span className={styles.label}>Discipline *</span>
                  <select
                    className={styles.input}
                    value={form.discipline}
                    onChange={e => setForm(f => ({ ...f, discipline: e.target.value as Discipline }))}
                    required
                  >
                    <option value="" disabled>Select…</option>
                    {DISCIPLINES.map(d => (
                      <option key={d} value={d}>{d}</option>
                    ))}
                  </select>
                </label>
              </div>

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

              <label className={styles.field}>
                <span className={styles.label}>Logo URL</span>
                <input
                  className={styles.input}
                  value={form.logoUrl}
                  onChange={e => setForm(f => ({ ...f, logoUrl: e.target.value }))}
                  placeholder="https://…"
                />
              </label>

              <label className={styles.field}>
                <span className={styles.label}>Documentation URL</span>
                <input
                  className={styles.input}
                  value={form.documentationUrl}
                  onChange={e => setForm(f => ({ ...f, documentationUrl: e.target.value }))}
                  placeholder="https://…"
                />
              </label>

              <label className={styles.checkField}>
                <input
                  type="checkbox"
                  checked={form.isFeatured}
                  onChange={e => setForm(f => ({ ...f, isFeatured: e.target.checked }))}
                />
                <span>Featured</span>
              </label>

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
