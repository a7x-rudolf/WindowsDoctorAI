import { useEffect, useState } from "react";
import { ChevronDown, ChevronRight, Play, ShieldCheck } from "lucide-react";
import { Topbar } from "../components/layout/Topbar";
import { Card, CardHeader, CardTitle, CardContent } from "../components/ui/card";
import { Badge, statusVariant } from "../components/ui/badge";
import { Button } from "../components/ui/button";
import { TerminalModal } from "../components/TerminalModal";
import { useActionRunner } from "../hooks/useActionRunner";
import { fetchAnalysis } from "../services/api";

export default function WindowsHealth() {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState(null);
  const [lastUpdated, setLastUpdated] = useState(null);
  const [expanded, setExpanded] = useState(null);
  const { session, runAction, closeSession } = useActionRunner();

  const load = async (isManual = false) => {
    isManual ? setRefreshing(true) : setLoading(true);
    setError(null);
    try {
      const result = await fetchAnalysis();
      setData(result);
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

  const diagnoses = data?.diagnoses ?? [];

  return (
    <div className="flex h-full flex-col">
      <Topbar
        title="Windows Health"
        subtitle="Event log diagnosis with one-click automated repair actions"
        overallHealth={(data?.overall_health ?? "unknown").toLowerCase()}
        connected={!error}
        onRefresh={() => load(true)}
        refreshing={refreshing}
        lastUpdated={lastUpdated}
      />

      <div className="flex-1 overflow-y-auto px-6 py-6">
        <TerminalModal session={session} onClose={closeSession} />

        {loading && <p className="text-text-muted">Scanning Windows event logs…</p>}
        {error && <p className="text-critical">{error}</p>}

        {!loading && !error && (
          <Card>
            <CardHeader>
              <CardTitle>Diagnosis Report</CardTitle>
              <Badge variant="neutral">{diagnoses.length} item{diagnoses.length !== 1 ? "s" : ""}</Badge>
            </CardHeader>
            <CardContent className="px-0 pb-0">
              {diagnoses.length === 0 ? (
                <div className="flex items-center gap-2 px-5 pb-5 text-[13px] text-good">
                  <ShieldCheck className="h-4 w-4" /> No diagnoses found — Windows looks healthy.
                </div>
              ) : (
                <table className="w-full text-[13px]">
                  <thead>
                    <tr className="border-y border-border text-left text-[11px] font-semibold uppercase tracking-wide text-text-faint">
                      <th className="px-5 py-2.5">Service</th>
                      <th className="px-5 py-2.5">Category</th>
                      <th className="px-5 py-2.5">Severity</th>
                      <th className="px-5 py-2.5">Count</th>
                      <th className="px-5 py-2.5">Status</th>
                      <th className="w-8 px-5 py-2.5" />
                    </tr>
                  </thead>
                  <tbody>
                    {diagnoses.map((d, i) => (
                      <RowGroup
                        key={i}
                        index={i}
                        d={d}
                        isOpen={expanded === i}
                        onToggle={() => setExpanded(expanded === i ? null : i)}
                        onRun={runAction}
                      />
                    ))}
                  </tbody>
                </table>
              )}
            </CardContent>
          </Card>
        )}
      </div>
    </div>
  );
}

function RowGroup({ d, isOpen, onToggle, onRun }) {
  return (
    <>
      <tr
        className="cursor-pointer border-b border-border/60 hover:bg-surface-raised"
        onClick={onToggle}
      >
        <td className="px-5 py-3 font-medium text-text">{d.service_name ?? "—"}</td>
        <td className="px-5 py-3 text-text-muted">{d.category ?? "—"}</td>
        <td className="px-5 py-3">
          <Badge variant={statusVariant(d.severity)}>{d.severity ?? "—"}</Badge>
        </td>
        <td className="px-5 py-3 text-text-muted">{d.count ?? "—"}</td>
        <td className="px-5 py-3">
          <Badge variant={statusVariant(d.service_health)}>{d.service_health ?? "Unknown"}</Badge>
        </td>
        <td className="px-5 py-3 text-text-faint">
          {isOpen ? <ChevronDown className="h-4 w-4" /> : <ChevronRight className="h-4 w-4" />}
        </td>
      </tr>

      {isOpen && (
        <tr className="border-b border-border bg-base/60">
          <td colSpan={6} className="px-5 py-4">
            <div className="mb-3 flex flex-wrap gap-5 border-b border-border pb-3 text-[11px] text-text-faint">
              {d.event_id && <span>Event ID: <strong className="text-text-muted">{d.event_id}</strong></span>}
              {d.provider && <span>Provider: <strong className="text-text-muted">{d.provider}</strong></span>}
              {d.current_status && (
                <span>
                  Current Status:{" "}
                  <strong className={d.current_status === "Running" ? "text-good" : "text-critical"}>
                    {d.current_status}
                  </strong>
                </span>
              )}
              {d.priority_score != null && <span>Priority Score: <strong className="text-text-muted">{d.priority_score}</strong></span>}
            </div>

            {d.possible_causes?.length > 0 && (
              <div className="mb-3">
                <div className="mb-1 text-[11px] font-semibold uppercase tracking-wide text-text-faint">
                  Possible Causes
                </div>
                <ul className="list-disc space-y-1 pl-5 text-[12px] text-text-muted">
                  {d.possible_causes.map((c, j) => <li key={j}>{c}</li>)}
                </ul>
              </div>
            )}

            {d.recommended_actions?.length > 0 && (
              <div>
                <div className="mb-1.5 text-[11px] font-semibold uppercase tracking-wide text-text-faint">
                  Recommended Actions
                </div>
                <div className="space-y-1.5">
                  {d.recommended_actions.map((action, j) => (
                    <div key={j} className="flex items-center justify-between rounded-lg border border-border bg-surface px-3 py-2">
                      <span className="text-[12px] text-text-muted">{action}</span>
                      <Button
                        variant="primary"
                        size="sm"
                        onClick={(e) => {
                          e.stopPropagation();
                          onRun(action, d.service_name);
                        }}
                      >
                        <Play className="h-3 w-3" /> Run
                      </Button>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </td>
        </tr>
      )}
    </>
  );
}
