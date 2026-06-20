import axios from "axios";

const API = axios.create({
  baseURL: "http://127.0.0.1:8000",
  timeout: 30000,
});

export default API;

// ── Convenience wrappers ────────────────────────────────────────────────────

export const fetchOverview = () => API.get("/overview").then((r) => r.data);

export const fetchAnalysis = () => API.get("/analysis").then((r) => r.data);
export const fetchDiagnosis = () => API.get("/diagnosis").then((r) => r.data);
export const fetchServices = () => API.get("/services").then((r) => r.data);

export const fetchHardwareDiagnosis = () =>
  API.get("/hardware/diagnosis").then((r) => r.data);
export const fetchHardwareCpu = () => API.get("/hardware/cpu").then((r) => r.data);
export const fetchHardwareMemory = () => API.get("/hardware/memory").then((r) => r.data);
export const fetchHardwareGpu = () => API.get("/hardware/gpu").then((r) => r.data);
export const fetchHardwareDisk = () => API.get("/hardware/disk").then((r) => r.data);
export const fetchHardwareDrivers = () => API.get("/hardware/drivers").then((r) => r.data);
export const fetchHardwareStartup = () => API.get("/hardware/startup").then((r) => r.data);
export const fetchHardwareProcesses = (sortBy = "cpu", limit = 10) =>
  API.get(`/hardware/processes?sort_by=${sortBy}&limit=${limit}`).then((r) => r.data);

export const fetchSystemInfo = () => API.get("/system/info").then((r) => r.data);

export const fetchScanHistory = (limit = 30) =>
  API.get(`/history/scans?limit=${limit}`).then((r) => r.data);
export const fetchActionHistory = (limit = 50) =>
  API.get(`/history/actions?limit=${limit}`).then((r) => r.data);

export const ACTION_ENDPOINT = `${API.defaults.baseURL}/action/run`;

export const fetchAIAnalysis = () => API.get("/ai/analyze").then((r) => r.data);
