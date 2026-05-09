export function formatDate(iso: string): string {
  // Accepts both 'yyyy-MM-dd' (DateOnly) and full ISO datetimes.
  const date = iso.length === 10 ? new Date(`${iso}T00:00:00Z`) : new Date(iso);
  return date.toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });
}

export function formatPrice(amount: number): string {
  return amount === 0
    ? 'Free'
    : new Intl.NumberFormat(undefined, { style: 'currency', currency: 'USD' }).format(amount);
}
