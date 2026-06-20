import { cn } from "../lib/utils.js";

/**
 * The app's signature element: an ECG-style waveform that doubles as the
 * literal "vitals" of the machine. Amplitude + color respond to health:
 *   good    -> calm, steady, teal
 *   warning -> slightly irregular, amber
 *   poor    -> sharp spikes, red
 */
export function VitalsLine({ status = "good", className }) {
  const color =
    status === "poor" ? "var(--color-critical)" :
    status === "warning" ? "var(--color-warn)" :
    "var(--color-vital)";

  const path =
    status === "poor"
      ? "M0,20 L20,20 L26,4 L32,36 L38,20 L48,20 L54,2 L60,38 L66,20 L90,20 L96,4 L102,36 L108,20 L160,20"
      : status === "warning"
      ? "M0,20 L30,20 L36,10 L42,28 L48,20 L90,20 L96,12 L102,26 L108,20 L160,20"
      : "M0,20 L60,20 L66,12 L72,28 L78,20 L160,20";

  return (
    <svg
      viewBox="0 0 160 40"
      preserveAspectRatio="none"
      className={cn("h-8 w-full", className)}
    >
      <path
        d={path}
        fill="none"
        stroke={color}
        strokeWidth="2"
        strokeLinecap="round"
        strokeLinejoin="round"
        opacity="0.85"
      />
    </svg>
  );
}
