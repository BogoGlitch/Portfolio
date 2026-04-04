'use client';

import { useEffect, useRef, useState, useCallback, useMemo } from 'react';
import { useRouter } from 'next/navigation';
import { TbSearch, TbCode, TbCpu, TbX } from 'react-icons/tb';
import styles from './CommandPalette.module.css';

type CommandItem = {
  type: 'project' | 'skill';
  name: string;
  slug: string;
  description?: string;
};

type CommandPaletteProps = {
  projects: CommandItem[];
  skills: CommandItem[];
};

export default function CommandPalette({ projects, skills }: CommandPaletteProps) {
  const router = useRouter();
  const [open, setOpen] = useState(false);
  const [query, setQuery] = useState('');
  const [activeIndex, setActiveIndex] = useState(0);
  const inputRef = useRef<HTMLInputElement>(null);
  const listRef = useRef<HTMLUListElement>(null);

  const allItems = useMemo<CommandItem[]>(
    () => [
      ...projects.map((p) => ({ ...p, type: 'project' as const })),
      ...skills.map((s) => ({ ...s, type: 'skill' as const })),
    ],
    [projects, skills],
  );

  const filtered = useMemo(() => {
    if (!query.trim()) return allItems;
    const q = query.toLowerCase();
    return allItems.filter(
      (item) =>
        item.name.toLowerCase().includes(q) ||
        item.description?.toLowerCase().includes(q),
    );
  }, [query, allItems]);

  const openPalette = useCallback(() => {
    setOpen(true);
    setQuery('');
    setActiveIndex(0);
  }, []);

  const closePalette = useCallback(() => {
    setOpen(false);
    setQuery('');
    setActiveIndex(0);
  }, []);

  const navigate = useCallback(
    (item: CommandItem) => {
      const path = item.type === 'project'
        ? `/projects/${item.slug}`
        : `/skills/${item.slug}`;
      router.push(path);
      closePalette();
    },
    [router, closePalette],
  );

  // Open on Cmd+K / Ctrl+K
  useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      if ((e.metaKey || e.ctrlKey) && e.key === 'k') {
        e.preventDefault();
        open ? closePalette() : openPalette();
      }
    };
    window.addEventListener('keydown', handler);
    return () => window.removeEventListener('keydown', handler);
  }, [open, openPalette, closePalette]);

  // Open via header button
  useEffect(() => {
    window.addEventListener('open-command-palette', openPalette);
    return () => window.removeEventListener('open-command-palette', openPalette);
  }, [openPalette]);

  // Focus input when opened
  useEffect(() => {
    if (open) {
      requestAnimationFrame(() => inputRef.current?.focus());
    }
  }, [open]);

  // Keyboard navigation inside palette
  useEffect(() => {
    if (!open) return;

    const handler = (e: KeyboardEvent) => {
      if (e.key === 'Escape') { closePalette(); return; }
      if (e.key === 'ArrowDown') {
        e.preventDefault();
        setActiveIndex((i) => Math.min(i + 1, filtered.length - 1));
      }
      if (e.key === 'ArrowUp') {
        e.preventDefault();
        setActiveIndex((i) => Math.max(i - 1, 0));
      }
      if (e.key === 'Enter' && filtered[activeIndex]) {
        navigate(filtered[activeIndex]);
      }
    };

    window.addEventListener('keydown', handler);
    return () => window.removeEventListener('keydown', handler);
  }, [open, filtered, activeIndex, navigate, closePalette]);

  // Scroll active item into view
  useEffect(() => {
    const item = listRef.current?.children[activeIndex] as HTMLElement | undefined;
    item?.scrollIntoView({ block: 'nearest' });
  }, [activeIndex]);

  // Reset active index when results change
  useEffect(() => { setActiveIndex(0); }, [filtered]);

  if (!open) return null;

  return (
    <div className={styles.overlay} onMouseDown={closePalette}>
      <div className={styles.palette} onMouseDown={(e) => e.stopPropagation()}>
        {/* Search input */}
        <div className={styles.inputRow}>
          <TbSearch size={18} className={styles.searchIcon} />
          <input
            ref={inputRef}
            type="text"
            className={styles.input}
            placeholder="Search projects and skills..."
            value={query}
            onChange={(e) => setQuery(e.target.value)}
          />
          <button className={styles.closeBtn} onClick={closePalette} aria-label="Close">
            <TbX size={16} />
          </button>
        </div>

        {/* Results */}
        <ul ref={listRef} className={styles.list} role="listbox">
          {filtered.length === 0 ? (
            <li className={styles.empty}>No results for &ldquo;{query}&rdquo;</li>
          ) : (
            filtered.map((item, i) => (
              <li
                key={`${item.type}-${item.slug}`}
                role="option"
                aria-selected={i === activeIndex}
                className={`${styles.item} ${i === activeIndex ? styles.active : ''}`}
                onMouseEnter={() => setActiveIndex(i)}
                onMouseDown={() => navigate(item)}
              >
                <span className={styles.itemIcon}>
                  {item.type === 'project' ? <TbCode size={16} /> : <TbCpu size={16} />}
                </span>
                <span className={styles.itemBody}>
                  <span className={styles.itemName}>{item.name}</span>
                  {item.description && (
                    <span className={styles.itemDesc}>{item.description}</span>
                  )}
                </span>
                <span className={styles.itemType}>{item.type}</span>
              </li>
            ))
          )}
        </ul>

        {/* Footer hint */}
        <div className={styles.footer}>
          <span><kbd>↑↓</kbd> navigate</span>
          <span><kbd>↵</kbd> open</span>
          <span><kbd>esc</kbd> close</span>
        </div>
      </div>
    </div>
  );
}
