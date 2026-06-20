import { useCallback, useEffect, useState } from "react";
import { fetchOverview } from "../services/api";

export function useOverview({ pollMs = 0 } = {}) {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState(null);
  const [lastUpdated, setLastUpdated] = useState(null);

  const load = useCallback(async (isManual = false) => {
    isManual ? setRefreshing(true) : setLoading(true);
    setError(null);
    try {
      const result = await fetchOverview();
      setData(result);
      setLastUpdated(new Date().toLocaleTimeString());
    } catch (err) {
      setError(
        err?.response?.data?.detail ??
        err?.message ??
        "Failed to connect to backend."
      );
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, []);

  useEffect(() => {
    load(false);
    if (pollMs > 0) {
      const id = setInterval(() => load(true), pollMs);
      return () => clearInterval(id);
    }
  }, [load, pollMs]);

  return {
    data,
    loading,
    refreshing,
    error,
    connected: !error,
    lastUpdated,
    refresh: () => load(true),
  };
}
