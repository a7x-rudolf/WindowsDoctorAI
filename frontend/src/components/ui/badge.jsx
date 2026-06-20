import { cva } from "class-variance-authority";
import { cn } from "../../lib/utils";

const badgeVariants = cva(
  "inline-flex items-center gap-1.5 rounded-md px-2 py-0.5 text-[11px] font-semibold uppercase tracking-wide",
  {
    variants: {
      variant: {
        neutral: "bg-surface-raised text-text-muted",
        good: "bg-good-dim text-good",
        warn: "bg-warn-dim text-warn",
        critical: "bg-critical-dim text-critical",
        info: "bg-info-dim text-info",
        vital: "bg-vital/15 text-vital",
      },
    },
    defaultVariants: {
      variant: "neutral",
    },
  }
);

export function Badge({ className, variant, ...props }) {
  return (
    <span className={cn(badgeVariants({ variant }), className)} {...props} />
  );
}

/** Map a severity/health string to a Badge variant automatically. */
export function statusVariant(value) {
  const v = (value || "").toLowerCase();
  if (["good", "healthy", "running", "recovered", "low"].includes(v)) return "good";
  if (["warning", "medium", "degraded"].includes(v)) return "warn";
  if (["poor", "high", "critical", "stopped", "still failing", "unhealthy"].includes(v)) return "critical";
  return "neutral";
}
