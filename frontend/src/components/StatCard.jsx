import { cn } from "../lib/utils";

export function StatCard({ label, value, sub, icon: Icon, accent = "text-vital" }) {
  return (
    <div className="flex items-center gap-4 rounded-xl border border-border bg-surface px-5 py-4">
      {Icon && (
        <div className={cn("flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-surface-raised", accent)}>
          <Icon className="h-5 w-5" strokeWidth={2} />
        </div>
      )}
      <div className="min-w-0">
        <div className="font-display text-2xl font-bold leading-tight text-text">{value}</div>
        <div className="text-[11px] font-semibold uppercase tracking-wider text-text-faint">
          {label}
        </div>
        {sub && <div className="mt-0.5 truncate text-[12px] text-text-muted">{sub}</div>}
      </div>
    </div>
  );
}