import { useEffect, useState } from "react";
import {
  LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer,
} from "recharts";
import { Topbar } from "../components/layout/Topbar";
import { Card, CardHeader, CardTitle, CardContent } from "../components/ui/card";
import { Badge, statusVariant } from "../components/ui/badge";
import { fetchScanHistory, fetchActionHistory } from "../services/api";

export default function History() {
  const [scans, setScans] = useState([]);
  const [actions, setActions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    (async () => {
      try {
        const [s, a] = await Promise.all([
          fetchScanHistory(30),
          fetchActionHistory(50),
        ]);
        setScans([...s.history].reverse());
        setActions(a.history);
      } catch (err) {
        setError(err?.message ?? "Failed to load history.");
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  const chartData = scans.map((s) => ({
    time: new Date(s.timestamp).toLocaleString([], { month: "short", day: "numeric", hour: "2-digit", minute: "2-digit" }),
    score: s.windows_health_score ?? 0,
  }));

  return (
    <div className="flex h-full flex-col">
      <Topbar
        title="History"
        subtitle="Health score trend & action execution log"
        connected={!error}
      />

      <div className="flex-1 overflow-y-auto px-6 py-6">
        {loading && <p className="text-text-muted">Loading history…</p>}
        {error && <p className="text-critical">{error}</p>}

        {!loading && !error && (
          <div className="space-y-5">
            <Card>
              <CardHeader>
                <CardTitle>Windows Health Score Trend</CardTitle>
              </CardHeader>
              <CardContent>
                {chartData.length === 0 ? (
                  <p className="text-[12px] text-text-faint">
                    No scan history yet — run a scan from the Overview page first.
                  </p>
                ) : (
                  <div className="h-64">
                    <ResponsiveContainer width="100%" height="100%">
                      <LineChart data={chartData}>
                        <CartesianGrid stroke="#1f2630" strokeDasharray="3 3" vertical={false} />
                        <XAxis dataKey="time" tick={{ fill: "#8b97a6", fontSize: 11 }} axisLine={{ stroke: "#1f2630" }} tickLine={false} />
                        <YAxis domain={[0, 100]} tick={{ fill: "#8b97a6", fontSize: 11 }} axisLine={{ stroke: "#1f2630" }} tickLine={false} width={32} />
                        <Tooltip
                          contentStyle={{ background: "#11161d", border: "1px solid #1f2630", borderRadius: 8, fontSize: 12 }}
                          labelStyle={{ color: "#8b97a6" }}
                        />
                        <Line type="monotone" dataKey="score" stroke="#2dd4bf" strokeWidth={2} dot={{ r: 3, fill: "#2dd4bf" }} />
                      </LineChart>
                    </ResponsiveContainer>
                  </div>
                )}
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Action Execution Log</CardTitle>
                <Badge variant="neutral">{actions.length}</Badge>
              </CardHeader>
              <CardContent className="px-0 pb-0">
                {actions.length === 0 ? (
                  <p className="px-5 pb-5 text-[12px] text-text-faint">No actions have been run yet.</p>
                ) : (
                  <table className="w-full text-[12px]">
                    <thead>
                      <tr className="border-y border-border text-left text-[11px] uppercase tracking-wide text-text-faint">
                        <th className="px-5 py-2.5">Time</th>
                        <th className="px-5 py-2.5">Action</th>
                        <th className="px-5 py-2.5">Service</th>
                        <th className="px-5 py-2.5">Status</th>
                      </tr>
                    </thead>
                    <tbody>
                      {actions.map((a) => (
                        <tr key={a.id} className="border-b border-border/50 last:border-0">
                          <td className="px-5 py-2.5 text-text-faint">
                            {new Date(a.timestamp).toLocaleString()}
                          </td>
                          <td className="px-5 py-2.5 text-text">{a.action}</td>
                          <td className="px-5 py-2.5 text-text-muted">{a.service_name || "—"}</td>
                          <td className="px-5 py-2.5">
                            <Badge variant={statusVariant(a.status === "success" ? "good" : "critical")}>
                              {a.status}
                            </Badge>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                )}
              </CardContent>
            </Card>
          </div>
        )}
      </div>
    </div>
  );
}
