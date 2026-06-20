import { useEffect, useRef, useState } from "react";
import API from "../services/api";
import Header from "../components/Header";
import Footer from "../components/Footer";

// ── Utility helpers ──────────────────────────────────────────────────────────

function healthColor(status) {
  if (!status) return "#64748b";
  const s = status.toLowerCase();
  if (s === "good") return "#22c55e";
  if (s === "warning") return "#f59e0b";
  if (s === "poor") return "#ef4444";
  return "#64748b";
}

function severityColor(sev) {
  if (sev === "high") return "#ef4444";
  if (sev === "medium") return "#f59e0b";
  return "#64748b";
}

function serviceHealthColor(sh) {
  if (sh === "Recovered") return "#22c55e";
  if (sh === "Still Failing") return "#ef4444";
  return "#94a3b8";
}

// ── Sub-components ───────────────────────────────────────────────────────────

function StatCard({ label, value, sub, accent }) {
  return (
    <div style={styles.statCard}>
      <div style={{ ...styles.statAccent, background: accent }} />
      <div style={styles.statContent}>
        <div style={styles.statValue}>{value}</div>
        <div style={styles.statLabel}>{label}</div>
        {sub && <div style={styles.statSub}>{sub}</div>}
      </div>
    </div>
  );
}

function SystemBanner({ status }) {
  const color = healthColor(status);
  return (
    <div style={{ ...styles.banner, borderColor: color }}>
      <div style={{ ...styles.bannerDot, background: color }} />
      <span style={styles.bannerText}>
        Overall System Health:&nbsp;
        <span style={{ color, fontWeight: 700 }}>{status ?? "—"}</span>
      </span>
    </div>
  );
}

function DiskCard({ disk }) {
  if (!disk) return null;
  const healthy = disk.healthy;
  const accent = healthy ? "#22c55e" : "#ef4444";

  // Hitung used_percent dari free_percent yang dikirim backend
  const freePercent = disk.free_percent ?? null;
  const usedPercent = freePercent !== null ? Math.round(100 - freePercent) : null;

  // Warna progress bar berdasarkan usage
  const barColor =
    usedPercent > 85 ? "#ef4444" :
    usedPercent > 70 ? "#f59e0b" :
    "#22c55e";

  return (
    <div style={styles.card}>
      <div style={styles.cardHeader}>
        <span style={styles.cardTitle}>Disk Status</span>
        <span style={{ ...styles.badge, background: accent + "22", color: accent }}>
          {healthy ? "Healthy" : "Degraded"}
        </span>
      </div>

      <p style={styles.diskMsg}>{disk.message}</p>

      {/* Detail row: Drive label */}
      {disk.drive && (
        <div style={styles.diskDriveLabel}>Drive {disk.drive}</div>
      )}

      {/* Stats grid */}
      <div style={styles.diskGrid}>
        <div style={styles.diskStat}>
          <div style={styles.diskStatValue}>{disk.total_gb ?? "—"} GB</div>
          <div style={styles.diskStatLabel}>Total</div>
        </div>
        <div style={styles.diskStat}>
          <div style={styles.diskStatValue}>{disk.used_gb ?? "—"} GB</div>
          <div style={styles.diskStatLabel}>Used</div>
        </div>
        <div style={styles.diskStat}>
          <div style={styles.diskStatValue}>{disk.free_gb ?? "—"} GB</div>
          <div style={styles.diskStatLabel}>Free</div>
        </div>
        <div style={styles.diskStat}>
          <div style={{ ...styles.diskStatValue, color: barColor }}>
            {freePercent !== null ? `${freePercent}%` : "—"}
          </div>
          <div style={styles.diskStatLabel}>Free %</div>
        </div>
      </div>

      {/* Progress bar */}
      {usedPercent !== null && (
        <div style={styles.diskBarWrap}>
          <div style={styles.diskBarTrack}>
            <div style={{
              ...styles.diskBarFill,
              width: `${usedPercent}%`,
              background: barColor,
            }} />
          </div>
          <div style={styles.diskBarLabels}>
            <span style={{ color: barColor }}>{usedPercent}% used</span>
            <span style={{ color: "#475569" }}>{freePercent}% free</span>
          </div>
        </div>
      )}
    </div>
  );
}

function CriticalIssues({ issues }) {
  return (
    <div style={styles.card}>
      <div style={styles.cardHeader}>
        <span style={styles.cardTitle}>Critical Issues</span>
        <span style={{
          ...styles.badge,
          background: issues.length > 0 ? "#ef444422" : "#22c55e22",
          color: issues.length > 0 ? "#ef4444" : "#22c55e"
        }}>
          {issues.length}
        </span>
      </div>
      {issues.length === 0 ? (
        <p style={styles.emptyMsg}>✓ No critical issues detected</p>
      ) : (
        <ul style={styles.issueList}>
          {issues.map((issue, i) => (
            <li key={i} style={styles.issueItem}>
              <span style={styles.issueDot} />
              <div>
                <div style={styles.issueName}>{issue.name}</div>
                <div style={styles.issueType}>{issue.type}</div>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

// ── Terminal Modal ────────────────────────────────────────────────────────────

function TerminalModal({ session, onClose }) {
  const bottomRef = useRef(null);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [session?.lines]);

  if (!session) return null;

  const isDone = session.lines.some(l => l.includes("[DONE]"));
  const hasError = session.lines.some(l => l.startsWith("[ERROR]"));

  return (
    <div style={styles.modalOverlay} onClick={onClose}>
      <div style={styles.modalBox} onClick={e => e.stopPropagation()}>

        {/* Header */}
        <div style={styles.modalHeader}>
          <div>
            <div style={styles.modalTitle}>▶ {session.action}</div>
            <div style={styles.modalSub}>{session.service}</div>
          </div>
          <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
            {!isDone && (
              <div style={styles.runningDot} />
            )}
            {isDone && (
              <span style={{
                fontSize: 11,
                fontWeight: 700,
                color: hasError ? "#ef4444" : "#22c55e",
                background: hasError ? "#ef444422" : "#22c55e22",
                padding: "3px 8px",
                borderRadius: 6,
              }}>
                {hasError ? "FAILED" : "DONE"}
              </span>
            )}
            <button style={styles.closeBtn} onClick={onClose}>✕</button>
          </div>
        </div>

        {/* Terminal body */}
        <div style={styles.terminal}>
          {session.lines.length === 0 && (
            <span style={{ color: "#475569" }}>Initializing…</span>
          )}
          {session.lines.map((line, i) => {
            const color =
              line.startsWith("[ERROR]") ? "#ef4444" :
              line.startsWith("[DONE]")  ? "#22c55e" :
              line.startsWith("[RUN]")   ? "#3b82f6" :
              line.startsWith("[INFO]")  ? "#f59e0b" :
              line.startsWith("[EXIT]")  ? "#94a3b8" :
              "#e2e8f0";
            return (
              <div key={i} style={{ color, marginBottom: 2 }}>
                {line}
              </div>
            );
          })}
          <div ref={bottomRef} />
        </div>

        {isDone && (
          <div style={styles.modalFooter}>
            <button style={styles.closeActionBtn} onClick={onClose}>
              Close
            </button>
          </div>
        )}
      </div>
    </div>
  );
}



function DiagnosisTable({ diagnoses }) {
  const [expanded, setExpanded] = useState(null);
  const [terminal, setTerminal] = useState(null);

  const runAction = (action, serviceName) => {
    setTerminal({ action, service: serviceName, lines: [] });

    fetch("http://127.0.0.1:8000/action/run", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ action, service_name: serviceName }),
    }).then(async (res) => {
      const reader = res.body.getReader();
      const decoder = new TextDecoder();
      let buffer = "";

      while (true) {
        const { done, value } = await reader.read();
        if (done) break;
        buffer += decoder.decode(value, { stream: true });
        const parts = buffer.split("\n\n");
        buffer = parts.pop();
        for (const part of parts) {
          const dataLine = part.split("\n").find(l => l.startsWith("data:"));
          if (dataLine) {
            const text = dataLine.replace(/^data:\s?/, "").replace(/↵/g, "\n");
            setTerminal(prev => prev
              ? { ...prev, lines: [...prev.lines, text] }
              : prev
            );
          }
        }
      }
    }).catch(err => {
      setTerminal(prev => prev
        ? { ...prev, lines: [...prev.lines, `[ERROR] ${err.message}`] }
        : prev
      );
    });
  };

  return (
    <>
      <TerminalModal session={terminal} onClose={() => setTerminal(null)} />
      <div style={styles.card}>
        <div style={styles.cardHeader}>
          <span style={styles.cardTitle}>Diagnosis Report</span>
          <span style={{ ...styles.badge, background: "#334155", color: "#94a3b8" }}>
            {diagnoses.length} item{diagnoses.length !== 1 ? "s" : ""}
          </span>
        </div>
        {diagnoses.length === 0 ? (
          <p style={styles.emptyMsg}>✓ No diagnoses found</p>
        ) : (
          <table style={styles.table}>
            <thead>
              <tr>
                {["Service", "Category", "Severity", "Count", "Status"].map((h) => (
                  <th key={h} style={styles.th}>{h}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {diagnoses.map((d, i) => (
                <>
                  <tr
                    key={i}
                    style={{ ...styles.trHover, cursor: "pointer" }}
                    onClick={() => setExpanded(expanded === i ? null : i)}
                  >
                    <td style={{ ...styles.td, fontWeight: 600, color: "#e2e8f0" }}>
                      {d.service_name ?? "—"}
                    </td>
                    <td style={styles.td}>{d.category ?? "—"}</td>
                    <td style={styles.td}>
                      <span style={{
                        ...styles.badge,
                        background: severityColor(d.severity) + "22",
                        color: severityColor(d.severity)
                      }}>
                        {d.severity ?? "—"}
                      </span>
                    </td>
                    <td style={styles.td}>{d.count ?? "—"}</td>
                    <td style={styles.td}>
                      <span style={{
                        ...styles.badge,
                        background: serviceHealthColor(d.service_health) + "22",
                        color: serviceHealthColor(d.service_health)
                      }}>
                        {d.service_health ?? "Unknown"}
                      </span>
                    </td>
                  </tr>
                  {expanded === i && (
                    <tr key={`exp-${i}`}>
                      <td colSpan={5} style={styles.expandedCell}>
                        {/* Metadata */}
                        <div style={styles.expandMeta}>
                          {d.event_id && <span style={styles.expandMetaItem}>Event ID: <strong>{d.event_id}</strong></span>}
                          {d.provider && <span style={styles.expandMetaItem}>Provider: <strong>{d.provider}</strong></span>}
                          {d.current_status && (
                            <span style={styles.expandMetaItem}>
                              Current Status:{" "}
                              <strong style={{ color: d.current_status === "Running" ? "#22c55e" : "#ef4444" }}>
                                {d.current_status}
                              </strong>
                            </span>
                          )}
                          {d.priority_score && <span style={styles.expandMetaItem}>Priority Score: <strong>{d.priority_score}</strong></span>}
                        </div>

                        {/* Possible Causes */}
                        {d.possible_causes?.length > 0 && (
                          <div style={styles.expandBlock}>
                            <div style={styles.expandLabel}>Possible Causes</div>
                            <ul style={styles.expandList}>
                              {d.possible_causes.map((c, j) => (
                                <li key={j}>{c}</li>
                              ))}
                            </ul>
                          </div>
                        )}

                        {/* Recommended Actions — with Run buttons */}
                        {d.recommended_actions?.length > 0 && (
                          <div style={styles.expandBlock}>
                            <div style={styles.expandLabel}>Recommended Actions</div>
                            <div style={styles.actionList}>
                              {d.recommended_actions.map((action, j) => (
                                <div key={j} style={styles.actionRow}>
                                  <span style={styles.actionName}>{action}</span>
                                  <button
                                    style={styles.actionBtn}
                                    onClick={(e) => {
                                      e.stopPropagation();
                                      runAction(action, d.service_name);
                                    }}
                                  >
                                    ▶ Run
                                  </button>
                                </div>
                              ))}
                            </div>
                          </div>
                        )}
                      </td>
                    </tr>
                  )}
                </>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </>
  );
}

// ── Main Dashboard ────────────────────────────────────────────────────────────

export default function Dashboard() {
  const [data, setData] = useState(null);
  const [diskRaw, setDiskRaw] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [lastRefresh, setLastRefresh] = useState(null);

  const fetchData = async () => {
    setLoading(true);
    setError(null);
    try {
      const [analysisRes, diskRes] = await Promise.all([
        API.get("/analysis"),
        API.get("/disk"),
      ]);
      setData(analysisRes.data);
      setDiskRaw(diskRes.data);
      setLastRefresh(new Date().toLocaleTimeString());
    } catch (err) {
      setError(
        err?.response?.data?.detail ??
        err?.message ??
        "Failed to connect to backend."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const healthScore =
    data?.overall_health?.toLowerCase() === "good"
      ? 98
      : data?.overall_health?.toLowerCase() === "warning"
      ? 65
      : 30;

  return (
    <div style={styles.page}>
      <div style={styles.container}>

        <Header />

        {/* Top row: stat cards */}
        <div style={styles.statRow}>
          <StatCard
            label="Health Score"
            value={loading ? "—" : healthScore}
            sub={data?.overall_health ?? "—"}
            accent={healthColor(data?.overall_health)}
          />
          <StatCard
            label="Disk Status"
            value={loading ? "—" : data?.disk?.healthy ? "Good" : "Issues"}
            sub={data?.disk?.message ?? "—"}
            accent={data?.disk?.healthy ? "#22c55e" : "#ef4444"}
          />
          <StatCard
            label="Critical Issues"
            value={loading ? "—" : data?.critical_issues?.length ?? 0}
            sub={
              data?.critical_issues?.length === 0
                ? "All clear"
                : "Needs attention"
            }
            accent={
              (data?.critical_issues?.length ?? 0) === 0 ? "#22c55e" : "#ef4444"
            }
          />
          <StatCard
            label="Diagnoses"
            value={loading ? "—" : data?.diagnoses?.length ?? 0}
            sub="Events analyzed"
            accent="#3b82f6"
          />
        </div>

        {/* System banner */}
        {!loading && !error && (
          <SystemBanner status={data?.overall_health} />
        )}

        {/* Loading */}
        {loading && (
          <div style={styles.stateBox}>
            <div style={styles.spinner} />
            <span style={styles.stateText}>Scanning system…</span>
          </div>
        )}

        {/* Error */}
        {error && (
          <div style={{ ...styles.stateBox, borderColor: "#ef4444" }}>
            <span style={{ color: "#ef4444", fontSize: 20 }}>⚠</span>
            <div>
              <div style={{ color: "#ef4444", fontWeight: 600 }}>Backend Unreachable</div>
              <div style={styles.stateText}>{error}</div>
            </div>
            <button style={styles.retryBtn} onClick={fetchData}>
              Retry
            </button>
          </div>
        )}

        {/* Main content */}
        {!loading && !error && data && (
          <>
            {/* 2-col: disk + critical issues */}
            <div style={styles.twoCol}>
              <DiskCard disk={{ ...data.disk, ...diskRaw }} />
              <CriticalIssues issues={data.critical_issues ?? []} />
            </div>

            {/* Diagnosis table */}
            <DiagnosisTable diagnoses={data.diagnoses ?? []} />
          </>
        )}

        {/* Footer row: refresh */}
        {!loading && !error && (
          <div style={styles.refreshRow}>
            <span style={styles.refreshTime}>Last scan: {lastRefresh}</span>
            <button style={styles.refreshBtn} onClick={fetchData}>
              ↻ Refresh
            </button>
          </div>
        )}

        <Footer />
      </div>
    </div>
  );
}

// ── Styles ────────────────────────────────────────────────────────────────────

const styles = {
  page: {
    minHeight: "100vh",
    background: "#0f172a",
    color: "#f8fafc",
    fontFamily: '"Segoe UI", system-ui, sans-serif',
  },
  container: {
    maxWidth: 1400,
    margin: "0 auto",
    padding: "24px",
  },

  // Stat cards row
  statRow: {
    display: "grid",
    gridTemplateColumns: "repeat(4, 1fr)",
    gap: 16,
    marginBottom: 16,
  },
  statCard: {
    background: "#1e293b",
    border: "1px solid #334155",
    borderRadius: 12,
    overflow: "hidden",
    display: "flex",
    flexDirection: "row",
  },
  statAccent: {
    width: 4,
    flexShrink: 0,
  },
  statContent: {
    padding: "18px 20px",
  },
  statValue: {
    fontSize: 32,
    fontWeight: 700,
    letterSpacing: "-0.5px",
    lineHeight: 1,
    marginBottom: 4,
  },
  statLabel: {
    fontSize: 12,
    fontWeight: 600,
    color: "#64748b",
    textTransform: "uppercase",
    letterSpacing: "0.06em",
    marginBottom: 2,
  },
  statSub: {
    fontSize: 12,
    color: "#94a3b8",
    marginTop: 4,
  },

  // Banner
  banner: {
    display: "flex",
    alignItems: "center",
    gap: 10,
    background: "#1e293b",
    border: "1px solid",
    borderRadius: 10,
    padding: "14px 20px",
    marginBottom: 16,
  },
  bannerDot: {
    width: 10,
    height: 10,
    borderRadius: "50%",
    flexShrink: 0,
  },
  bannerText: {
    fontSize: 14,
    color: "#cbd5e1",
  },

  // Generic card
  card: {
    background: "#1e293b",
    border: "1px solid #334155",
    borderRadius: 12,
    padding: "20px",
    marginBottom: 16,
  },
  cardHeader: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: 16,
  },
  cardTitle: {
    fontSize: 14,
    fontWeight: 700,
    color: "#e2e8f0",
    textTransform: "uppercase",
    letterSpacing: "0.05em",
  },
  badge: {
    fontSize: 11,
    fontWeight: 600,
    padding: "3px 8px",
    borderRadius: 6,
  },

  // Two column layout
  twoCol: {
    display: "grid",
    gridTemplateColumns: "1fr 1fr",
    gap: 16,
  },

  // Table
  table: {
    width: "100%",
    borderCollapse: "collapse",
    fontSize: 13,
  },
  th: {
    textAlign: "left",
    padding: "8px 12px",
    color: "#64748b",
    fontSize: 11,
    fontWeight: 600,
    textTransform: "uppercase",
    letterSpacing: "0.05em",
    borderBottom: "1px solid #334155",
  },
  td: {
    padding: "10px 12px",
    color: "#cbd5e1",
    borderBottom: "1px solid #1e293b",
  },
  trHover: {
    transition: "background 0.15s",
  },

  // Expanded row
  expandedCell: {
    background: "#0f172a",
    padding: "14px 20px",
    borderBottom: "1px solid #334155",
  },
  expandMeta: {
    display: "flex",
    gap: 20,
    flexWrap: "wrap",
    marginBottom: 12,
    paddingBottom: 10,
    borderBottom: "1px solid #1e293b",
  },
  expandMetaItem: {
    fontSize: 11,
    color: "#64748b",
  },
  expandBlock: {
    marginBottom: 10,
  },
  expandLabel: {
    fontSize: 11,
    fontWeight: 700,
    color: "#64748b",
    textTransform: "uppercase",
    letterSpacing: "0.05em",
    marginBottom: 4,
  },
  expandList: {
    paddingLeft: 18,
    color: "#94a3b8",
    fontSize: 12,
    lineHeight: 1.7,
  },

  // Disk
  diskMsg: {
    fontSize: 13,
    color: "#94a3b8",
    marginBottom: 14,
  },
  diskDriveLabel: {
    fontSize: 11,
    fontWeight: 700,
    color: "#475569",
    textTransform: "uppercase",
    letterSpacing: "0.06em",
    marginBottom: 10,
  },
  diskGrid: {
    display: "grid",
    gridTemplateColumns: "repeat(4, 1fr)",
    gap: 12,
    marginBottom: 16,
  },
  diskStat: {
    background: "#0f172a",
    borderRadius: 8,
    padding: "10px 12px",
  },
  diskStatValue: {
    fontSize: 16,
    fontWeight: 700,
    color: "#e2e8f0",
    marginBottom: 2,
  },
  diskStatLabel: {
    fontSize: 10,
    fontWeight: 600,
    color: "#475569",
    textTransform: "uppercase",
    letterSpacing: "0.05em",
  },
  diskBarWrap: {
    marginTop: 4,
  },
  diskBarTrack: {
    height: 6,
    background: "#0f172a",
    borderRadius: 99,
    overflow: "hidden",
    marginBottom: 6,
  },
  diskBarFill: {
    height: "100%",
    borderRadius: 99,
    transition: "width 0.6s ease",
  },
  diskBarLabels: {
    display: "flex",
    justifyContent: "space-between",
    fontSize: 11,
    color: "#475569",
  },

  // Critical issues
  emptyMsg: {
    fontSize: 13,
    color: "#22c55e",
    padding: "8px 0",
  },
  issueList: {
    listStyle: "none",
    padding: 0,
    margin: 0,
  },
  issueItem: {
    display: "flex",
    alignItems: "flex-start",
    gap: 10,
    padding: "10px 0",
    borderBottom: "1px solid #334155",
  },
  issueDot: {
    width: 8,
    height: 8,
    borderRadius: "50%",
    background: "#ef4444",
    marginTop: 4,
    flexShrink: 0,
  },
  issueName: {
    fontSize: 13,
    color: "#e2e8f0",
    fontWeight: 600,
  },
  issueType: {
    fontSize: 11,
    color: "#64748b",
    marginTop: 2,
  },

  // State boxes
  stateBox: {
    display: "flex",
    alignItems: "center",
    gap: 14,
    background: "#1e293b",
    border: "1px solid #334155",
    borderRadius: 12,
    padding: "20px 24px",
    marginBottom: 16,
  },
  stateText: {
    color: "#94a3b8",
    fontSize: 13,
  },
  spinner: {
    width: 20,
    height: 20,
    border: "2px solid #334155",
    borderTopColor: "#3b82f6",
    borderRadius: "50%",
    animation: "spin 0.8s linear infinite",
  },
  retryBtn: {
    marginLeft: "auto",
    background: "#334155",
    color: "#f8fafc",
    border: "none",
    borderRadius: 8,
    padding: "8px 16px",
    fontSize: 13,
    cursor: "pointer",
  },

  // Refresh row
  refreshRow: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: 16,
  },
  refreshTime: {
    fontSize: 12,
    color: "#475569",
  },
  refreshBtn: {
    background: "#1e293b",
    border: "1px solid #334155",
    color: "#94a3b8",
    borderRadius: 8,
    padding: "6px 14px",
    fontSize: 12,
    cursor: "pointer",
  },

  // Action buttons in diagnosis expand
  actionList: {
    display: "flex",
    flexDirection: "column",
    gap: 6,
    marginTop: 4,
  },
  actionRow: {
    display: "flex",
    alignItems: "center",
    justifyContent: "space-between",
    background: "#1e293b",
    borderRadius: 8,
    padding: "8px 12px",
    border: "1px solid #334155",
  },
  actionName: {
    fontSize: 12,
    color: "#cbd5e1",
  },
  actionBtn: {
    background: "#1d4ed8",
    color: "#fff",
    border: "none",
    borderRadius: 6,
    padding: "5px 12px",
    fontSize: 11,
    fontWeight: 700,
    cursor: "pointer",
    letterSpacing: "0.04em",
    transition: "background 0.15s",
  },

  // Terminal modal
  modalOverlay: {
    position: "fixed",
    inset: 0,
    background: "rgba(0,0,0,0.75)",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    zIndex: 1000,
  },
  modalBox: {
    width: "min(800px, 90vw)",
    background: "#0f172a",
    border: "1px solid #334155",
    borderRadius: 14,
    overflow: "hidden",
    display: "flex",
    flexDirection: "column",
    maxHeight: "80vh",
  },
  modalHeader: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    padding: "14px 20px",
    background: "#1e293b",
    borderBottom: "1px solid #334155",
  },
  modalTitle: {
    fontSize: 13,
    fontWeight: 700,
    color: "#e2e8f0",
    fontFamily: '"Courier New", monospace',
  },
  modalSub: {
    fontSize: 11,
    color: "#64748b",
    marginTop: 2,
  },
  runningDot: {
    width: 8,
    height: 8,
    borderRadius: "50%",
    background: "#22c55e",
    animation: "pulse 1s ease-in-out infinite",
  },
  closeBtn: {
    background: "transparent",
    border: "none",
    color: "#64748b",
    fontSize: 16,
    cursor: "pointer",
    padding: "2px 6px",
  },
  terminal: {
    flex: 1,
    overflowY: "auto",
    padding: "16px 20px",
    fontFamily: '"Courier New", Courier, monospace',
    fontSize: 12,
    lineHeight: 1.7,
    background: "#0f172a",
    minHeight: 200,
    maxHeight: "60vh",
  },
  modalFooter: {
    padding: "12px 20px",
    borderTop: "1px solid #1e293b",
    display: "flex",
    justifyContent: "flex-end",
  },
  closeActionBtn: {
    background: "#334155",
    color: "#f8fafc",
    border: "none",
    borderRadius: 8,
    padding: "8px 20px",
    fontSize: 13,
    cursor: "pointer",
  },
};