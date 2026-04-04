'use client';

import { useState, useMemo } from 'react';
import { useRouter } from 'next/navigation';
import type { Skill } from '@/types/skill';
import styles from './FilterModal.module.css';

type Props = {
  skills: Skill[];
  currentSkillIds: string[];
  basePath: string;
};

const DISCIPLINE_ORDER = ['Frontend', 'Backend', 'Database', 'Cloud', 'DevOps', 'AI'];

export default function ProjectFilterModal({ skills, currentSkillIds, basePath }: Props) {
  const router = useRouter();
  const [open, setOpen] = useState(false);
  const [pendingDisciplines, setPendingDisciplines] = useState<string[]>([]);
  const [pendingCategories, setPendingCategories] = useState<string[]>([]);
  const [pendingSkillIds, setPendingSkillIds] = useState<string[]>([]);

  const openModal = () => {
    const selectedSkills = skills.filter(s => currentSkillIds.includes(String(s.id)));
    setPendingDisciplines([...new Set(selectedSkills.map(s => s.discipline))]);
    setPendingCategories([...new Set(
      selectedSkills
        .filter((s): s is typeof s & { category: string } => s.category !== null)
        .map(s => `${s.discipline}::${s.category}`),
    )]);
    setPendingSkillIds(currentSkillIds);
    setOpen(true);
  };

  // For each selected discipline, which categories are available
  const categoriesByDiscipline = useMemo(() => {
    const result: Record<string, string[]> = {};
    for (const d of pendingDisciplines) {
      const cats = [...new Set(
        skills.filter(s => s.discipline === d && s.category !== null).map(s => s.category as string),
      )].sort();
      if (cats.length > 0) result[d] = cats;
    }
    return result;
  }, [skills, pendingDisciplines]);

  // For each selected discipline::category composite key, which skills are available
  const skillsByCategory = useMemo(() => {
    const result: Record<string, Skill[]> = {};
    for (const key of pendingCategories) {
      const [discipline, category] = key.split('::');
      const matching = skills.filter(
        s => s.discipline === discipline && s.category === category,
      );
      if (matching.length > 0) result[key] = matching;
    }
    return result;
  }, [skills, pendingCategories]);

  const toggleDiscipline = (discipline: string) => {
    const next = pendingDisciplines.includes(discipline)
      ? pendingDisciplines.filter(d => d !== discipline)
      : [...pendingDisciplines, discipline];

    const nextCategories = pendingCategories.filter(key => next.includes(key.split('::')[0]));
    const validComposites = new Set(nextCategories);
    const nextSkillIds = pendingSkillIds.filter(id => {
      const skill = skills.find(s => String(s.id) === id);
      return skill ? validComposites.has(`${skill.discipline}::${skill.category}`) : false;
    });

    setPendingDisciplines(next);
    setPendingCategories(nextCategories);
    setPendingSkillIds(nextSkillIds);
  };

  const toggleCategory = (discipline: string, category: string) => {
    const key = `${discipline}::${category}`;
    const next = pendingCategories.includes(key)
      ? pendingCategories.filter(k => k !== key)
      : [...pendingCategories, key];

    const validComposites = new Set(next);
    const nextSkillIds = pendingSkillIds.filter(id => {
      const skill = skills.find(s => String(s.id) === id);
      return skill ? validComposites.has(`${skill.discipline}::${skill.category}`) : false;
    });

    setPendingCategories(next);
    setPendingSkillIds(nextSkillIds);
  };

  const toggleSkill = (skillId: string) => {
    setPendingSkillIds(prev =>
      prev.includes(skillId) ? prev.filter(id => id !== skillId) : [...prev, skillId],
    );
  };

  const clearAll = () => {
    setPendingDisciplines([]);
    setPendingCategories([]);
    setPendingSkillIds([]);
  };

  const apply = () => {
    const prev = [...currentSkillIds].sort().join(',');
    const next = [...pendingSkillIds].sort().join(',');
    if (prev !== next) {
      const query = pendingSkillIds.length > 0 ? `?skillIds=${pendingSkillIds.join(',')}` : '';
      router.push(`${basePath}${query}`);
    }
    setOpen(false);
  };

  const orderedDisciplines = DISCIPLINE_ORDER.filter(d => skills.some(s => s.discipline === d));
  const hasDisciplines = pendingDisciplines.length > 0;
  const hasCategories = pendingCategories.length > 0;
  const activeCount = currentSkillIds.length;
  const hasChanged = [...currentSkillIds].sort().join(',') !== [...pendingSkillIds].sort().join(',');

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

              {/* ── Level 3: Skill ── */}
              <div className={`${styles.group} ${!hasCategories ? styles.groupDimmed : ''}`}>
                <div className={styles.groupHeader}>
                  <span className={styles.groupLabel}>Skill</span>
                  <span className={styles.groupHelp}>(Choose 1 or more.)</span>
                </div>
                {!hasCategories ? (
                  <div className={styles.groupCardWaiting}>
                    <span className={styles.groupCardEmpty}>Select a category above to see skills.</span>
                  </div>
                ) : (
                  <div className={styles.groupCardsWrap}>
                    {pendingCategories.map(key => {
                      const [, category] = key.split('::');
                      return (
                      <div key={key} className={styles.groupCard}>
                        <div className={styles.groupCardLabel}>{category}</div>
                        <div className={styles.groupItems}>
                          {(skillsByCategory[key] ?? []).map(s => (
                            <button
                              key={s.id}
                              className={`${styles.item} ${pendingSkillIds.includes(String(s.id)) ? styles.itemActive : ''}`}
                              onClick={() => toggleSkill(String(s.id))}
                            >
                              {s.name}
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
                {hasChanged ? `Apply (${pendingSkillIds.length})` : 'Close'}
              </button>
            </div>
          </div>
        </>
      )}
    </>
  );
}
