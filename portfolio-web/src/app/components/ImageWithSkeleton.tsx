'use client';

import Image from 'next/image';
import { useState } from 'react';

type Props = {
  src: string;
  alt: string;
  fill?: boolean;
  sizes?: string;
  priority?: boolean;
  /** CSS module class applied to the wrapper div (provides sizing/shape). */
  className?: string;
  /** CSS module class applied to the Next.js Image element. */
  imgClassName?: string;
};

/**
 * Drop-in replacement for a `<div><Image fill /></div>` pattern.
 * Shows the global `.skeleton` shimmer on the wrapper until the image loads,
 * then fades the image in and removes the shimmer.
 */
export default function ImageWithSkeleton({
  src,
  alt,
  fill,
  sizes,
  priority,
  className,
  imgClassName,
}: Props) {
  const [loaded, setLoaded] = useState(false);

  const wrapperClass = [loaded ? '' : 'skeleton', className ?? '']
    .filter(Boolean)
    .join(' ');

  return (
    <div className={wrapperClass}>
      <Image
        src={src}
        alt={alt}
        fill={fill}
        sizes={sizes}
        priority={priority}
        className={imgClassName}
        style={{ opacity: loaded ? 1 : 0, transition: 'opacity 0.3s ease' }}
        onLoad={() => setLoaded(true)}
      />
    </div>
  );
}
