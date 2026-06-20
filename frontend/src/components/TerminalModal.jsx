import { useEffect, useRef } from "react";
import { X, Terminal } from "lucide-react";
import { cn } from "../lib/utils.js";
import { Button } from "./ui/button";

export function TerminalModal({ session, onClose }) {
  const bottomRef = useRef(null);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [session?.lines]);

  if (!session) return null;

  const isDone = session.lines.some((l) => l.includes("[DONE]"));
  const hasError = session.lines.some((l) => l.startsWith("[ERROR]"));

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 p-4"
      onClick={onClose}
    >
      <div
        className="flex max-h-[80vh] w-full max-w-2xl flex-col overflow-hidden rounded-xl border border-border bg-base shadow-2xl"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="flex items-center justify-between border-b border-border bg-surface px-5 py-3.5">
          <div className="flex items-center gap-2.5">
            <Terminal className="h-4 w-4 text-vital" />
            <div>
              <div className="font-mono text-[13px] font-semibold text-text">
                {session.action}
              </div>
              <div className="text-[11px] text-text-faint">{session.service}</div>
            </div>
          </div>
          <div className="flex items-center gap-3">
            {!isDone && <span className="h-2 w-2 rounded-full bg-good animate-pulse-dot" />}
            {isDone && (
              <span
                className={cn(
                  "rounded-md px-2 py-0.5 text-[11px] font-bold",
                  hasError ? "bg-critical-dim text-critical" : "bg-good-dim text-good"
                )}
              >
                {hasError ? "FAILED" : "DONE"}
              </span>
            )}
            <button onClick={onClose} className="text-text-faint hover:text-text">
              <X className="h-4 w-4" />
            </button>
          </div>
        </div>

        <div className="min-h-[200px] flex-1 overflow-y-auto px-5 py-4 font-mono text-[12px] leading-relaxed">
          {session.lines.length === 0 && (
            <span className="text-text-faint">Initializing…</span>
          )}
          {session.lines.map((line, i) => (
            <div key={i} className={cn("mb-0.5", lineColor(line))}>
              {line}
            </div>
          ))}
          <div ref={bottomRef} />
        </div>

        {isDone && (
          <div className="flex justify-end border-t border-border px-5 py-3">
            <Button variant="secondary" size="sm" onClick={onClose}>
              Close
            </Button>
          </div>
        )}
      </div>
    </div>
  );
}

function lineColor(line) {
  if (line.startsWith("[ERROR]")) return "text-critical";
  if (line.startsWith("[DONE]")) return "text-good";
  if (line.startsWith("[RUN]")) return "text-info";
  if (line.startsWith("[INFO]")) return "text-warn";
  if (line.startsWith("[EXIT]")) return "text-text-faint";
  return "text-text-muted";
}
