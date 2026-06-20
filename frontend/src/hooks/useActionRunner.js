import { useState, useCallback } from "react";
import { ACTION_ENDPOINT } from "../services/api";

export function useActionRunner() {
  const [session, setSession] = useState(null); // { action, service, lines }

  const runAction = useCallback((action, serviceName) => {
    setSession({ action, service: serviceName, lines: [] });

    fetch(ACTION_ENDPOINT, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ action, service_name: serviceName }),
    })
      .then(async (res) => {
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
            const dataLine = part.split("\n").find((l) => l.startsWith("data:"));
            if (dataLine) {
              const text = dataLine.replace(/^data:\s?/, "").replace(/↵/g, "\n");
              setSession((prev) =>
                prev ? { ...prev, lines: [...prev.lines, text] } : prev
              );
            }
          }
        }
      })
      .catch((err) => {
        setSession((prev) =>
          prev ? { ...prev, lines: [...prev.lines, `[ERROR] ${err.message}`] } : prev
        );
      });
  }, []);

  const closeSession = useCallback(() => setSession(null), []);

  return { session, runAction, closeSession };
}
