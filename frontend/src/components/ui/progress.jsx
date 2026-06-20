import { cn } from "../../lib/utils";

export function ProgressBar({ value = 0, className, barClassName, colorFor }) {
  const pct = Math.max(0, Math.min(100, value));
  const color = colorFor ? colorFor(pct) : "bg-vital";

  return (
    <div className={cn("h-1.5 w-full overflow-hidden rounded-full bg-surface-raised", className)}>
      <div
        className={cn("h-full rounded-full transition-all duration-500", color, barClassName)}
        style={{ width: `${pct}%` }}
      />
    </div>
  );
}

export function usageColor(pct) {
  if (pct >= 85) return "bg-critical";
  if (pct >= 65) return "bg-warn";
  return "bg-good";
}
