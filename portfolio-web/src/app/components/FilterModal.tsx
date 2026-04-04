'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import styles from './FilterModal.module.css';

export type FilterGroup = {
  /** Visual section heading (e.g. "Frontend", "Discipline") */
  label: string;
  /** URL query-param key this group writes to (e.g. "skillIds", "discipline") */
  paramName: string;
  /** true → checkbox multi-select; false → radio single-select */
  multiSelect: boolean;
  items: { value: string; label: string }[];
};

type FilterModalProps = {
  groups: FilterGroup[];
  /** Current URL-derived selections: Record<paramName, string[]> */
  current: Record<string, string[]>;
  basePath: string;
};

export default function FilterModal({ groups, current, basePath }: FilterModalProps) {
  const router = useRouter();
  const [open, setOpen] = useState(false);
  const [pending, setPending] = useState<Record<string, string[]>>(current);

  const activeCount = Object.values(current).reduce((n, arr) => n + arr.length, 0);
  const pendingCount = Object.values(pending).reduce((n, arr) => n + arr.length, 0);

  const openModal = () => {
    setPending(current);
    setOpen(true);
  };

  const close = () => setOpen(false);

  const toggle = (paramName: string, value: string, multiSelect: boolean) => {
    setPending(prev => {
      const cur = prev[paramName] ?? [];
      let next: string[];
      if (multiSelect) {
        next = cur.includes(value) ? cur.filter(v => v !== value) : [...cur, value];
      } else {
        // radio: clicking the active value deselects it
        next = cur[0] === value ? [] : [value];
      }
      return { ...prev, [paramName]: next };
    });
  };

  const clearAll = () => {
    const empty: Record<string, string[]> = {};
    for (const g of groups) empty[g.paramName] = [];
    setPending(empty);
  };

  const apply = () => {
    const parts: string[] = [];
    for (const [key, values] of Object.entries(pending)) {
      if (values.length > 0) parts.push(`${key}=${values.join(',')}`);
    }
    const query = parts.length > 0 ? `?${parts.join('&')}` : '';
    router.push(`${basePath}${query}`);
    close();
  };

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
          <div className={styles.overlay} onClick={close} />
          <div className={styles.modal} role="dialog" aria-modal="true" aria-label="Filters">
            <div className={styles.modalHeader}>
              <span className={styles.modalTitle}>Filters</span>
              <button className={styles.closeBtn} onClick={close} aria-label="Close filters">✕</button>
            </div>

            <div className={styles.modalBody}>
              {groups.map((group) => {
                const selected = pending[group.paramName] ?? [];
                return (
                  <div key={`${group.paramName}-${group.label}`} className={styles.group}>
                    <div className={styles.groupLabel}>{group.label}</div>
                    <div className={styles.groupItems}>
                      {group.items.map((item) => {
                        const active = selected.includes(item.value);
                        return (
                          <button
                            key={item.value}
                            className={`${styles.item} ${active ? styles.itemActive : ''}`}
                            onClick={() => toggle(group.paramName, item.value, group.multiSelect)}
                          >
                            {item.label}
                          </button>
                        );
                      })}
                    </div>
                  </div>
                );
              })}
            </div>

            <div className={styles.modalFooter}>
              <button className={styles.clearBtn} onClick={clearAll}>Clear all</button>
              <button className={styles.applyBtn} onClick={apply}>
                Apply{pendingCount > 0 ? ` (${pendingCount})` : ''}
              </button>
            </div>
          </div>
        </>
      )}
    </>
  );
}
