import { NavLink } from "react-router-dom";
import {
  LayoutDashboard,
  MonitorCheck,
  Cpu,
  History,
  Stethoscope,
} from "lucide-react";
import { cn } from "../../lib/utils";

const NAV_ITEMS = [
  { to: "/", label: "Overview", icon: LayoutDashboard },
  { to: "/windows", label: "Windows Health", icon: MonitorCheck },
  { to: "/hardware", label: "Hardware", icon: Cpu },
  { to: "/history", label: "History", icon: History },
];

export function Sidebar() {
  return (
    <aside className="flex h-full w-60 shrink-0 flex-col border-r border-border bg-surface">
      <div className="flex items-center gap-2.5 px-5 py-5">
        <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-vital/15">
          <Stethoscope className="h-5 w-5 text-vital" strokeWidth={2} />
        </div>
        <div className="leading-tight">
          <div className="font-display text-[15px] font-semibold text-text">
            Windows Doctor
          </div>
          <div className="text-[11px] font-medium uppercase tracking-wider text-text-faint">
            AI Diagnostics
          </div>
        </div>
      </div>

      <nav className="flex-1 space-y-1 px-3">
        {NAV_ITEMS.map(({ to, label, icon: Icon }) => (
          <NavLink
            key={to}
            to={to}
            end={to === "/"}
            className={({ isActive }) =>
              cn(
                "flex items-center gap-3 rounded-lg px-3 py-2.5 text-[13px] font-medium transition-colors",
                isActive
                  ? "bg-vital/10 text-vital"
                  : "text-text-muted hover:bg-surface-raised hover:text-text"
              )
            }
          >
            <Icon className="h-[17px] w-[17px]" strokeWidth={2} />
            {label}
          </NavLink>
        ))}
      </nav>

      <div className="border-t border-border px-5 py-4">
        <div className="text-[11px] text-text-faint">
          Windows Doctor AI v2.0
        </div>
        <div className="text-[11px] text-text-faint">Developed by Rudolf</div>
      </div>
    </aside>
  );
}