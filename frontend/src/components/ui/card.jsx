import { cn } from "../../lib/utils";

export function Card({ className, ...props }) {
  return (
    <div
      className={cn(
        "rounded-xl border border-border bg-surface",
        className
      )}
      {...props}
    />
  );
}

export function CardHeader({ className, ...props }) {
  return (
    <div
      className={cn(
        "flex items-center justify-between px-5 pt-5 pb-3",
        className
      )}
      {...props}
    />
  );
}

export function CardTitle({ className, ...props }) {
  return (
    <h3
      className={cn(
        "font-display text-[13px] font-semibold uppercase tracking-wider text-text-muted",
        className
      )}
      {...props}
    />
  );
}

export function CardContent({ className, ...props }) {
  return <div className={cn("px-5 pb-5", className)} {...props} />;
}
