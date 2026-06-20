import { HashRouter, Routes, Route } from "react-router-dom";
import { Sidebar } from "./components/layout/Sidebar";
import Overview from "./pages/Overview";
import WindowsHealth from "./pages/WindowsHealth";
import Hardware from "./pages/Hardware";
import History from "./pages/History";

function App() {
  return (
    <HashRouter>
      <div className="flex h-screen w-screen overflow-hidden bg-base">
        <Sidebar />
        <main className="flex-1 overflow-hidden">
          <Routes>
            <Route path="/" element={<Overview />} />
            <Route path="/windows" element={<WindowsHealth />} />
            <Route path="/hardware" element={<Hardware />} />
            <Route path="/history" element={<History />} />
          </Routes>
        </main>
      </div>
    </HashRouter>
  );
}

export default App;
