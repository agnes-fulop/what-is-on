import { useFormStatus } from 'react-dom';

interface SubmitButtonProps {
  idleLabel: string;
  pendingLabel?: string;
}

/**
 * Reads the parent form's pending state via React 19's useFormStatus so the
 * button is automatically disabled while the action is running. No manual
 * isLoading state needed in the parent.
 */
export function SubmitButton({ idleLabel, pendingLabel }: SubmitButtonProps) {
  const { pending } = useFormStatus();
  return (
    <button type="submit" className="btn btn--primary" disabled={pending}>
      {pending ? (pendingLabel ?? `${idleLabel}…`) : idleLabel}
    </button>
  );
}
