import { useEffect, useState } from "react";
import {
  Cpu, MemoryStick, MonitorPlay, HardDrive, PlugZap, Rocket, ListTree, Info,
} from "lucide-react";
import { Topbar } from "../components/layout/Topbar";
import { Card, CardHeader, CardTitle, CardContent } from "../components/ui/card";
import { Badge, statusVariant } from "../components/ui/badge";
import { ProgressBar, usageColor } from "../components/ui/progress";
import {
  fetchHardwareDiagnosis, fetchHardwareStartup, fetchHardwareProcesses,
} from "../services/api";

export default function Hardware() {
  const [diag, setDiag] = useState(null);
  const [startup, setStartup] = useState(null);
  const [processes, setProcesses] = useState(null);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState(null);
  const [lastUpdated, setLastUpdated] = useState(null);

  const load = async (isManual = false) => {
    isManual ? setRefreshing(true) : setLoading(true);
    setError(null);
    try {
      const [d, s, p] = await Promise.all([
        fetchHardwareDiagnosis(),
        fetchHardwareStartup(),
        fetchHardwareProcesses("cpu", 8),
      ]);
      setDiag(d);
      setStartup(s);
      setProcesses(p);
      setLastUpdated(new Date().toLocaleTimeString());
    } catch (err) {
      setError(err?.message ?? "Failed to connect to backend.");
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  useEffect(() => {
    load(false);
  }, []);

  const raw = diag?.raw ?? {};
  const cpu = raw.cpu;
  const mem = raw.memory;
  const gpu = raw.gpu;
  const disk = raw.disk;
  const drivers = raw.drivers;

  return (
    <div className="flex h-full flex-col">
      <Topbar
        title="Hardware Diagnostics"
        subtitle="Advisory only — Windows Doctor AI recommends, you decide on physical fixes"
        overallHealth={(diag?.overall_hardware_health ?? "unknown").toLowerCase()}
        connected={!error}
        onRefresh={() => load(true)}
        refreshing={refreshing}
        lastUpdated={lastUpdated}
      />

      <div className="flex-1 overflow-y-auto px-6 py-6">
        {loading && <p className="text-text-muted">Reading hardware sensors…</p>}
        {error && <p className="text-critical">{error}</p>}

        {!loading && !error && (
          <div className="space-y-5">
            {/* Advisory disclaimer banner */}
            <div className="flex items-start gap-3 rounded-xl border border-vital/30 bg-vital/5 px-4 py-3 text-[12px] text-text-muted">
              <Info className="mt-0.5 h-4 w-4 shrink-0 text-vital" />
              <p>
                Hardware findings are <strong className="text-text">recommendations only</strong>.
                Windows Doctor AI does not modify physical hardware automatically — apply fixes
                yourself or contact a technician based on these suggestions.
              </p>
            </div>

            {/* Findings list */}
            <Card>
              <CardHeader>
                <CardTitle>Findings</CardTitle>
                <Badge variant={diag?.total_findings > 0 ? "warn" : "good"}>
                  {diag?.total_findings ?? 0}
                </Badge>
              </CardHeader>
              <CardContent>
                {(!diag?.findings || diag.findings.length === 0) ? (
                  <p className="text-[13px] text-good">No hardware concerns detected.</p>
                ) : (
                  <div className="space-y-3">
                    {diag.findings.map((f, i) => (
                      <div key={i} className="rounded-lg border border-border bg-base/40 p-4">
                        <div className="mb-1.5 flex items-center justify-between">
                          <div className="flex items-center gap-2">
                            <span className="text-[13px] font-semibold text-text">{f.title}</span>
                            <Badge variant="neutral">{f.category}</Badge>
                          </div>
                          <Badge variant={statusVariant(f.severity)}>{f.severity}</Badge>
                        </div>
                        {f.detail && <p className="mb-2 text-[12px] text-text-faint">{f.detail}</p>}
                        <p className="mb-2 text-[12px] text-text-muted">{f.description}</p>
                        <ul className="list-disc space-y-1 pl-5 text-[12px] text-text-muted">
                          {f.recommended_actions.map((a, j) => <li key={j}>{a}</li>)}
                        </ul>
                      </div>
                    ))}
                  </div>
                )}
              </CardContent>
            </Card>

            {/* Component grid */}
            <div className="grid grid-cols-1 gap-5 lg:grid-cols-2">
              <SectionCard icon={Cpu} title="CPU">
                {cpu ? (
                  <>
                    <KV label="Model" value={cpu.name} />
                    <KV label="Cores" value={`${cpu.physical_cores} physical / ${cpu.logical_cores} logical`} />
                    <KV label="Frequency" value={cpu.current_freq_mhz ? `${cpu.current_freq_mhz} MHz` : "—"} />
                    <KV label="Temperature" value={cpu.temperature_c != null ? `${cpu.temperature_c}°C` : "Not available"} />
                    <UsageMeter label="Usage" value={cpu.usage_percent} />
                  </>
                ) : <NoData />}
              </SectionCard>

              <SectionCard icon={MemoryStick} title="Memory">
                {mem ? (
                  <>
                    <KV label="Total" value={`${mem.total_gb} GB`} />
                    <KV label="Used" value={`${mem.used_gb} GB`} />
                    <KV label="Swap" value={`${mem.swap_used_gb} / ${mem.swap_total_gb} GB`} />
                    <UsageMeter label="Usage" value={mem.usage_percent} />
                  </>
                ) : <NoData />}
              </SectionCard>

              <SectionCard icon={MonitorPlay} title="GPU">
                {gpu?.gpus?.length ? (
                  gpu.gpus.map((g, i) => (
                    <div key={i} className="mb-3 border-b border-border pb-3 last:mb-0 last:border-0 last:pb-0">
                      <KV label="Model" value={g.name} />
                      {g.memory_total_mb != null && <KV label="VRAM" value={`${Math.round(g.memory_total_mb)} MB`} />}
                      {g.temperature_c != null && <KV label="Temperature" value={`${g.temperature_c}°C`} />}
                      {g.utilization_percent != null && <UsageMeter label="Usage" value={g.utilization_percent} />}
                    </div>
                  ))
                ) : <NoData text="No GPU data detected" />}
              </SectionCard>

              <SectionCard icon={PlugZap} title="Driver / Device Health">
                {drivers?.success ? (
                  drivers.total_problems === 0 ? (
                    <p className="text-[13px] text-good">All devices reporting normal status.</p>
                  ) : (
                    <div className="space-y-2">
                      {drivers.problem_devices.slice(0, 5).map((p, i) => (
                        <div key={i} className="rounded-lg bg-base/40 px-3 py-2">
                          <div className="text-[12px] font-medium text-text">{p.name}</div>
                          <div className="text-[11px] text-text-faint">{p.error_meaning} (code {p.error_code})</div>
                        </div>
                      ))}
                    </div>
                  )
                ) : <NoData />}
              </SectionCard>
            </div>

            {/* Disk detail */}
            <Card>
              <CardHeader>
                <CardTitle>Disk Reliability (SMART)</CardTitle>
                <HardDrive className="h-4 w-4 text-text-faint" />
              </CardHeader>
              <CardContent>
                {disk?.disks?.length ? (
                  <table className="w-full text-[12px]">
                    <thead>
                      <tr className="border-b border-border text-left text-[11px] uppercase tracking-wide text-text-faint">
                        <th className="py-2 pr-4">Disk</th>
                        <th className="py-2 pr-4">Type</th>
                        <th className="py-2 pr-4">Size</th>
                        <th className="py-2 pr-4">Health</th>
                        <th className="py-2 pr-4">Wear</th>
                        <th className="py-2 pr-4">Temp</th>
                      </tr>
                    </thead>
                    <tbody>
                      {disk.disks.map((d, i) => (
                        <tr key={i} className="border-b border-border/50 last:border-0">
                          <td className="py-2 pr-4 text-text">{d.friendly_name}</td>
                          <td className="py-2 pr-4 text-text-muted">{d.media_type}</td>
                          <td className="py-2 pr-4 text-text-muted">{d.size_gb ? `${d.size_gb} GB` : "—"}</td>
                          <td className="py-2 pr-4">
                            <Badge variant={statusVariant(d.health_status)}>{d.health_status}</Badge>
                          </td>
                          <td className="py-2 pr-4 text-text-muted">{d.wear_percent != null ? `${d.wear_percent}%` : "—"}</td>
                          <td className="py-2 pr-4 text-text-muted">{d.temperature_c != null ? `${d.temperature_c}°C` : "—"}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                ) : <NoData text="Physical disk data not available" />}
              </CardContent>
            </Card>

            {/* Startup & processes */}
            <div className="grid grid-cols-1 gap-5 lg:grid-cols-2">
              <SectionCard icon={Rocket} title="Startup Impact">
                {startup?.success && startup.programs.length > 0 ? (
                  <div className="space-y-2">
                    {startup.programs.slice(0, 8).map((p, i) => (
                      <div key={i} className="flex items-center justify-between border-b border-border/50 pb-2 last:border-0 last:pb-0">
                        <span className="truncate text-[12px] text-text-muted">{p.name}</span>
                        <Badge variant={
                          p.estimated_impact === "High" ? "warn" :
                          p.estimated_impact === "Low" ? "good" : "neutral"
                        }>
                          {p.estimated_impact}
                        </Badge>
                      </div>
                    ))}
                  </div>
                ) : <NoData text="No startup program data" />}
              </SectionCard>

              <SectionCard icon={ListTree} title="Top Processes (CPU)">
                {processes?.top_processes?.length ? (
                  <div className="space-y-2">
                    {processes.top_processes.map((p) => (
                      <div key={p.pid} className="flex items-center justify-between border-b border-border/50 pb-2 last:border-0 last:pb-0">
                        <span className="truncate text-[12px] text-text-muted">{p.name}</span>
                        <span className="font-mono text-[12px] text-text">{p.cpu_percent}%</span>
                      </div>
                    ))}
                  </div>
                ) : <NoData />}
              </SectionCard>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

function SectionCard({ icon: Icon, title, children }) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>{title}</CardTitle>
        <Icon className="h-4 w-4 text-text-faint" />
      </CardHeader>
      <CardContent className="space-y-2">{children}</CardContent>
    </Card>
  );
}

function KV({ label, value }) {
  return (
    <div className="flex items-center justify-between text-[12px]">
      <span className="text-text-faint">{label}</span>
      <span className="truncate pl-3 text-right text-text-muted">{value ?? "—"}</span>
    </div>
  );
}

function UsageMeter({ label, value }) {
  return (
    <div className="pt-1.5">
      <div className="mb-1 flex items-center justify-between text-[12px]">
        <span className="text-text-faint">{label}</span>
        <span className="font-mono text-text-muted">{value ?? 0}%</span>
      </div>
      <ProgressBar value={value ?? 0} colorFor={usageColor} />
    </div>
  );
}

function NoData({ text = "No data available" }) {
  return <p className="text-[12px] text-text-faint">{text}</p>;
}
