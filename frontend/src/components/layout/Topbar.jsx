import { RefreshCw, Wifi, WifiOff } from "lucide-react";
import { VitalsLine } from "../VitalsLine";
import { Button } from "../ui/button";
import { cn } from "../../lib/utils";

export function Topbar({ title, subtitle, overallHealth, connected, onRefresh, refreshing, lastUpdated }) {
  const statusKey = (overallHealth || "").toLowerCase();
  const dotColor =
    statusKey === "poor" ? "bg-critical" :
    statusKey === "warning" ? "bg-warn" :
    statusKey === "good" ? "bg-good" : "bg-text-faint";

  return (
    <header className="flex items-center justify-between gap-6 border-b border-border bg-surface px-6 py-4">
      <div>
        <h1 className="font-display text-lg font-semibold text-text">{title}</h1>
        {subtitle && <p className="text-[12px] text-text-muted">{subtitle}</p>}
      </div>

      <div className="hidden w-48 shrink-0 md:block">
        <VitalsLine status={statusKey} />
      </div>

      <div className="flex items-center gap-4">
        <div className="flex items-center gap-2 text-[12px] text-text-muted">
          {connected ? (
            <Wifi className="h-3.5 w-3.5 text-good" />
          ) : (
            <WifiOff className="h-3.5 w-3.5 text-critical" />
          )}
          {connected ? "Backend connected" : "Backend offline"}
        </div>

        <div className="flex items-center gap-1.5">
          <span className={cn("h-2 w-2 rounded-full animate-pulse-dot", dotColor)} />
          <span className="text-[12px] font-medium text-text-muted">
            {overallHealth ?? "—"}
          </span>
        </div>

        {lastUpdated && (
          <span className="hidden text-[11px] text-text-faint lg:inline">
            Updated {lastUpdated}
          </span>
        )}

        <Button variant="secondary" size="sm" onClick={onRefresh} disabled={refreshing}>
          <RefreshCw className={cn("h-3.5 w-3.5", refreshing && "animate-spin")} />
          {refreshing ? "Scanning…" : "Rescan"}
        </Button>
      </div>
    </header>
  );
}