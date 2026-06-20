import { Link } from "react-router-dom";
import {
  ShieldCheck,
  AlertTriangle,
  HardDrive,
  Cpu,
  MemoryStick,
  Activity,
  ArrowUpRight,
} from "lucide-react";
import { Topbar } from "../components/layout/Topbar";
import { StatCard } from "../components/StatCard";
import { Card, CardHeader, CardTitle, CardContent } from "../components/ui/card";
import { Badge, statusVariant } from "../components/ui/badge";
import { ProgressBar, usageColor } from "../components/ui/progress";
import { RadialGauge } from "../components/ui/radial-gauge";
import { useOverview } from "../hooks/useOverview";

export default function Overview() {
  const { data, loading, refreshing, error, connected, lastUpdated, refresh } =
    useOverview({ pollMs: 30000 });

  const windowsHealth = data?.windows?.overall_health ?? "Unknown";
  const hardwareHealth = data?.hardware?.overall_hardware_health ?? "Unknown";
  const score = data?.windows?.health_score ?? 0;
  const criticalCount = data?.windows?.critical_issues?.length ?? 0;
  const findingsCount = data?.hardware?.total_findings ?? 0;

  const overallStatus =
    windowsHealth.toLowerCase() === "poor" || hardwareHealth.toLowerCase() === "poor"
      ? "poor"
      : windowsHealth.toLowerCase() === "warning" || hardwareHealth.toLowerCase() === "warning"
      ? "warning"
      : "good";

  const cpu = data?.hardware?.cpu;
  const mem = data?.hardware?.memory;

  return (
    <div className="flex h-full flex-col">
      <Topbar
        title="System Overview"
        subtitle="Real-time Windows & hardware diagnostic summary"
        overallHealth={overallStatus}
        connected={connected}
        onRefresh={refresh}
        refreshing={refreshing}
        lastUpdated={lastUpdated}
      />

      <div className="flex-1 overflow-y-auto px-6 py-6">
        {loading && (
          <div className="flex items-center gap-3 rounded-xl border border-border bg-surface px-5 py-4 text-text-muted">
            <Activity className="h-4 w-4 animate-pulse text-vital" />
            Running full system scan…
          </div>
        )}

        {error && !loading && (
          <Card className="mb-6 border-critical/40">
            <CardContent className="flex items-center justify-between py-4">
              <div className="flex items-center gap-3">
                <AlertTriangle className="h-5 w-5 text-critical" />
                <div>
                  <div className="font-semibold text-critical">Backend unreachable</div>
                  <div className="text-[12px] text-text-muted">{error}</div>
                </div>
              </div>
              <button
                onClick={refresh}
                className="rounded-lg border border-border-strong px-3 py-1.5 text-[12px] font-semibold text-text hover:bg-surface-raised"
              >
                Retry
              </button>
            </CardContent>
          </Card>
        )}

        {!loading && !error && data && (
          <>
            {/* Top stat row */}
            <div className="mb-6 grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-4">
              <StatCard
                label="Windows Health"
                value={`${score ?? "—"}`}
                sub={windowsHealth}
                icon={ShieldCheck}
                accent={
                  windowsHealth.toLowerCase() === "good" ? "text-good" :
                  windowsHealth.toLowerCase() === "warning" ? "text-warn" : "text-critical"
                }
              />
              <StatCard
                label="Hardware Health"
                value={hardwareHealth}
                sub={`${findingsCount} advisory finding${findingsCount !== 1 ? "s" : ""}`}
                icon={Cpu}
                accent={
                  hardwareHealth.toLowerCase() === "good" ? "text-good" :
                  hardwareHealth.toLowerCase() === "warning" ? "text-warn" : "text-critical"
                }
              />
              <StatCard
                label="Critical Issues"
                value={criticalCount}
                sub={criticalCount === 0 ? "All clear" : "Needs attention"}
                icon={AlertTriangle}
                accent={criticalCount === 0 ? "text-good" : "text-critical"}
              />
              <StatCard
                label="Uptime"
                value={data?.system?.uptime_readable ?? "—"}
                sub={data?.system?.computer_name ?? "—"}
                icon={Activity}
                accent="text-info"
              />
            </div>

            <div className="grid grid-cols-1 gap-5 lg:grid-cols-3">
              {/* Health score gauge */}
              <Card>
                <CardHeader>
                  <CardTitle>Windows Diagnosis Score</CardTitle>
                  <Link to="/windows" className="flex items-center gap-1 text-[11px] font-semibold text-vital hover:underline">
                    Details <ArrowUpRight className="h-3 w-3" />
                  </Link>
                </CardHeader>
                <CardContent className="flex items-center justify-center py-2">
                  <RadialGauge
                    value={score ?? 0}
                    label={windowsHealth}
                    color={
                      windowsHealth.toLowerCase() === "good" ? "#34d399" :
                      windowsHealth.toLowerCase() === "warning" ? "#f0a83c" : "#f0506b"
                    }
                  />
                </CardContent>
              </Card>

              {/* CPU / RAM live usage */}
              <Card>
                <CardHeader>
                  <CardTitle>Live Resource Usage</CardTitle>
                  <Link to="/hardware" className="flex items-center gap-1 text-[11px] font-semibold text-vital hover:underline">
                    Details <ArrowUpRight className="h-3 w-3" />
                  </Link>
                </CardHeader>
                <CardContent className="space-y-4">
                  <UsageRow icon={Cpu} label="CPU" value={cpu?.usage_percent ?? 0}
                    sub={cpu?.name} />
                  <UsageRow icon={MemoryStick} label="Memory" value={mem?.usage_percent ?? 0}
                    sub={`${mem?.used_gb ?? "—"} / ${mem?.total_gb ?? "—"} GB`} />
                  {cpu?.temperature_c != null && (
                    <div className="flex items-center justify-between text-[12px] text-text-muted">
                      <span>CPU Temperature</span>
                      <span className="font-mono text-text">{cpu.temperature_c}°C</span>
                    </div>
                  )}
                </CardContent>
              </Card>

              {/* Disk overview */}
              <Card>
                <CardHeader>
                  <CardTitle>Disk Status</CardTitle>
                  <HardDrive className="h-4 w-4 text-text-faint" />
                </CardHeader>
                <CardContent className="space-y-3">
                  {(data?.hardware?.disk?.volumes ?? []).slice(0, 3).map((v) => {
                    const usedPct = Math.round(100 - (v.free_percent ?? 0));
                    return (
                      <div key={v.drive}>
                        <div className="mb-1 flex items-center justify-between text-[12px]">
                          <span className="font-mono text-text">{v.drive}</span>
                          <span className="text-text-muted">{v.free_gb} GB free</span>
                        </div>
                        <ProgressBar value={usedPct} colorFor={usageColor} />
                      </div>
                    );
                  })}
                  {(!data?.hardware?.disk?.volumes || data.hardware.disk.volumes.length === 0) && (
                    <p className="text-[12px] text-text-faint">No volume data available.</p>
                  )}
                </CardContent>
              </Card>
            </div>

            {/* Two-column: critical issues + hardware findings */}
            <div className="mt-5 grid grid-cols-1 gap-5 lg:grid-cols-2">
              <Card>
                <CardHeader>
                  <CardTitle>Critical Windows Issues</CardTitle>
                  <Badge variant={criticalCount > 0 ? "critical" : "good"}>{criticalCount}</Badge>
                </CardHeader>
                <CardContent>
                  {criticalCount === 0 ? (
                    <EmptyState text="No critical issues detected" />
                  ) : (
                    <ul className="space-y-2.5">
                      {data.windows.critical_issues.map((issue, i) => (
                        <li key={i} className="flex items-start gap-3 border-b border-border pb-2.5 last:border-0 last:pb-0">
                          <span className="mt-1.5 h-1.5 w-1.5 shrink-0 rounded-full bg-critical" />
                          <div>
                            <div className="text-[13px] font-medium text-text">{issue.name}</div>
                            <div className="text-[11px] uppercase tracking-wide text-text-faint">{issue.type}</div>
                          </div>
                        </li>
                      ))}
                    </ul>
                  )}
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle>Hardware Advisory</CardTitle>
                  <Badge variant={findingsCount > 0 ? "warn" : "good"}>{findingsCount}</Badge>
                </CardHeader>
                <CardContent>
                  {findingsCount === 0 ? (
                    <EmptyState text="No hardware concerns found" />
                  ) : (
                    <ul className="space-y-2.5">
                      {data.hardware.findings.slice(0, 5).map((f, i) => (
                        <li key={i} className="flex items-start justify-between gap-3 border-b border-border pb-2.5 last:border-0 last:pb-0">
                          <div>
                            <div className="text-[13px] font-medium text-text">{f.title}</div>
                            <div className="text-[11px] text-text-faint">{f.detail}</div>
                          </div>
                          <Badge variant={statusVariant(f.severity)}>{f.severity}</Badge>
                        </li>
                      ))}
                    </ul>
                  )}
                  {findingsCount > 5 && (
                    <Link to="/hardware" className="mt-3 inline-flex items-center gap-1 text-[12px] font-semibold text-vital hover:underline">
                      View all {findingsCount} findings <ArrowUpRight className="h-3 w-3" />
                    </Link>
                  )}
                </CardContent>
              </Card>
            </div>
          </>
        )}
      </div>
    </div>
  );
}

function UsageRow({ icon: Icon, label, value, sub }) {
  return (
    <div>
      <div className="mb-1.5 flex items-center justify-between">
        <div className="flex items-center gap-2 text-[12px] font-medium text-text">
          <Icon className="h-3.5 w-3.5 text-text-faint" />
          {label}
        </div>
        <span className="font-mono text-[13px] font-semibold text-text">{value}%</span>
      </div>
      <ProgressBar value={value} colorFor={usageColor} />
      {sub && <div className="mt-1 truncate text-[11px] text-text-faint">{sub}</div>}
    </div>
  );
}

function EmptyState({ text }) {
  return (
    <div className="flex items-center gap-2 py-2 text-[13px] text-good">
      <ShieldCheck className="h-4 w-4" />
      {text}
    </div>
  );
}
