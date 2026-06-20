import { cva } from "class-variance-authority";
import { cn } from "../../lib/utils";

const buttonVariants = cva(
  "inline-flex items-center justify-center gap-2 rounded-lg text-[13px] font-semibold transition-colors disabled:opacity-40 disabled:cursor-not-allowed",
  {
    variants: {
      variant: {
        primary: "bg-vital-600 text-[#04201c] hover:bg-vital",
        secondary: "bg-surface-raised text-text border border-border-strong hover:bg-surface-hover",
        ghost: "text-text-muted hover:text-text hover:bg-surface-raised",
        danger: "bg-critical/15 text-critical hover:bg-critical/25",
      },
      size: {
        sm: "h-8 px-3",
        md: "h-9 px-4",
        icon: "h-9 w-9",
      },
    },
    defaultVariants: {
      variant: "secondary",
      size: "md",
    },
  }
);

export function Button({ className, variant, size, ...props }) {
  return (
    <button
      className={cn(buttonVariants({ variant, size }), className)}
      {...props}
    />
  );
}
