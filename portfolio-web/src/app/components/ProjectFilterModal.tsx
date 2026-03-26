'use client';

import { useState, useMemo } from 'react';
import { useRouter } from 'next/navigation';
import type { Technology } from '@/types/technology';
import styles from './FilterModal.module.css';

type Props = {
  technologies: Technology[];
  currentTechIds: string[];
  basePath: string;
};

const DISCIPLINE_ORDER = ['Frontend', 'Backend', 'Database', 'Cloud', 'DevOps', 'AI'];

export default function ProjectFilterModal({ technologies, currentTechIds, basePath }: Props) {
  const router = useRouter();
  const [open, setOpen] = useState(false);
  const [pendingDisciplines, setPendingDisciplines] = useState<string[]>([]);
  const [pendingCategories, setPendingCategories] = useState<string[]>([]);
  const [pendingTechIds, setPendingTechIds] = useState<string[]>([]);

  const openModal = () => {
    const selectedTechs = technologies.filter(t => currentTechIds.includes(String(t.id)));
    setPendingDisciplines([...new Set(selectedTechs.map(t => t.discipline))]);
    setPendingCategories([...new Set(
      selectedTechs
        .filter((t): t is typeof t & { category: string } => t.category !== null)
        .map(t => `${t.discipline}::${t.category}`),
    )]);
    setPendingTechIds(currentTechIds);
    setOpen(true);
  };

  // For each selected discipline, which categories are available
  const categoriesByDiscipline = useMemo(() => {
    const result: Record<string, string[]> = {};
    for (const d of pendingDisciplines) {
      const cats = [...new Set(
        technologies.filter(t => t.discipline === d && t.category !== null).map(t => t.category as string),
      )].sort();
      if (cats.length > 0) result[d] = cats;
    }
    return result;
  }, [technologies, pendingDisciplines]);

  // For each selected discipline::category composite key, which technologies are available
  const techsByCategory = useMemo(() => {
    const result: Record<string, Technology[]> = {};
    for (const key of pendingCategories) {
      const [discipline, category] = key.split('::');
      const techs = technologies.filter(
        t => t.discipline === discipline && t.category === category,
      );
      if (techs.length > 0) result[key] = techs;
    }
    return result;
  }, [technologies, pendingCategories]);

  const toggleDiscipline = (discipline: string) => {
    const next = pendingDisciplines.includes(discipline)
      ? pendingDisciplines.filter(d => d !== discipline)
      : [...pendingDisciplines, discipline];

    const nextCategories = pendingCategories.filter(key => next.includes(key.split('::')[0]));
    const validComposites = new Set(nextCategories);
    const nextTechIds = pendingTechIds.filter(id => {
      const tech = technologies.find(t => String(t.id) === id);
      return tech ? validComposites.has(`${tech.discipline}::${tech.category}`) : false;
    });

    setPendingDisciplines(next);
    setPendingCategories(nextCategories);
    setPendingTechIds(nextTechIds);
  };

  const toggleCategory = (discipline: string, category: string) => {
    const key = `${discipline}::${category}`;
    const next = pendingCategories.includes(key)
      ? pendingCategories.filter(k => k !== key)
      : [...pendingCategories, key];

    const validComposites = new Set(next);
    const nextTechIds = pendingTechIds.filter(id => {
      const tech = technologies.find(t => String(t.id) === id);
      return tech ? validComposites.has(`${tech.discipline}::${tech.category}`) : false;
    });

    setPendingCategories(next);
    setPendingTechIds(nextTechIds);
  };

  const toggleTech = (techId: string) => {
    setPendingTechIds(prev =>
      prev.includes(techId) ? prev.filter(id => id !== techId) : [...prev, techId],
    );
  };

  const clearAll = () => {
    setPendingDisciplines([]);
    setPendingCategories([]);
    setPendingTechIds([]);
  };

  const apply = () => {
    const prev = [...currentTechIds].sort().join(',');
    const next = [...pendingTechIds].sort().join(',');
    if (prev !== next) {
      const query = pendingTechIds.length > 0 ? `?technologyIds=${pendingTechIds.join(',')}` : '';
      router.push(`${basePath}${query}`);
    }
    setOpen(false);
  };

  const orderedDisciplines = DISCIPLINE_ORDER.filter(d => technologies.some(t => t.discipline === d));
  const hasDisciplines = pendingDisciplines.length > 0;
  const hasCategories = pendingCategories.length > 0;
  const activeCount = currentTechIds.length;
  const hasChanged = [...currentTechIds].sort().join(',') !== [...pendingTechIds].sort().join(',');

  return (
    <>
      <button
        className={`${styles.trigger} ${activeCount > 0 ? styles.triggerActive : ''}`}
        onClick={openModal}
      >
        <span>Filters</span>
        {activeCount > 0 && <span className={styles.badge}>{activeCount}</span>}
      </button>

      {open && (
        <>
          <div className={styles.overlay} onClick={apply} />
          <div className={styles.modal} role="dialog" aria-modal="true" aria-label="Filter projects">
            <div className={styles.modalHeader}>
              <span className={styles.modalTitle}>Filters</span>
              <button className={styles.closeBtn} onClick={apply} aria-label="Close filters">✕</button>
            </div>

            <div className={styles.modalBody}>

              {/* ── Level 1: Discipline ── */}
              <div className={styles.group}>
                <div className={styles.groupHeader}>
                  <span className={styles.groupLabel}>Discipline</span>
                  <span className={styles.groupHelp}>(Choose 1 or more.)</span>
                </div>
                <div className={styles.groupCard}>
                  <div className={styles.groupItems}>
                    {orderedDisciplines.map(d => (
                      <button
                        key={d}
                        className={`${styles.item} ${pendingDisciplines.includes(d) ? styles.itemActive : ''}`}
                        onClick={() => toggleDiscipline(d)}
                      >
                        {d}
                      </button>
                    ))}
                  </div>
                </div>
              </div>

              {/* ── Level 2: Category ── */}
              <div className={`${styles.group} ${!hasDisciplines ? styles.groupDimmed : ''}`}>
                <div className={styles.groupHeader}>
                  <span className={styles.groupLabel}>Category</span>
                  <span className={styles.groupHelp}>(Choose 1 or more.)</span>
                </div>
                {!hasDisciplines ? (
                  <div className={styles.groupCardWaiting}>
                    <span className={styles.groupCardEmpty}>Select a discipline above to see categories.</span>
                  </div>
                ) : (
                  <div className={styles.groupCards}>
                    {pendingDisciplines.map(d => (
                      <div key={d} className={styles.groupCard}>
                        <div className={styles.groupCardLabel}>{d}</div>
                        <div className={styles.groupItems}>
                          {(categoriesByDiscipline[d] ?? []).map(c => (
                            <button
                              key={c}
                              className={`${styles.item} ${pendingCategories.includes(`${d}::${c}`) ? styles.itemActive : ''}`}
                              onClick={() => toggleCategory(d, c)}
                            >
                              {c}
                            </button>
                          ))}
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>

              {/* ── Level 3: Technology ── */}
              <div className={`${styles.group} ${!hasCategories ? styles.groupDimmed : ''}`}>
                <div className={styles.groupHeader}>
                  <span className={styles.groupLabel}>Technology</span>
                  <span className={styles.groupHelp}>(Choose 1 or more.)</span>
                </div>
                {!hasCategories ? (
                  <div className={styles.groupCardWaiting}>
                    <span className={styles.groupCardEmpty}>Select a category above to see technologies.</span>
                  </div>
                ) : (
                  <div className={styles.groupCardsWrap}>
                    {pendingCategories.map(key => {
                      const [, category] = key.split('::');
                      return (
                      <div key={key} className={styles.groupCard}>
                        <div className={styles.groupCardLabel}>{category}</div>
                        <div className={styles.groupItems}>
                          {(techsByCategory[key] ?? []).map(t => (
                            <button
                              key={t.id}
                              className={`${styles.item} ${pendingTechIds.includes(String(t.id)) ? styles.itemActive : ''}`}
                              onClick={() => toggleTech(String(t.id))}
                            >
                              {t.name}
                            </button>
                          ))}
                        </div>
                      </div>
                      );
                    })}
                  </div>
                )}
              </div>

            </div>

            <div className={styles.modalFooter}>
              <button className={styles.clearBtn} onClick={clearAll} disabled={pendingDisciplines.length === 0}>Clear all</button>
              <button className={styles.applyBtn} onClick={apply}>
                {hasChanged ? `Apply (${pendingTechIds.length})` : 'Close'}
              </button>
            </div>
          </div>
        </>
      )}
    </>
  );
}
