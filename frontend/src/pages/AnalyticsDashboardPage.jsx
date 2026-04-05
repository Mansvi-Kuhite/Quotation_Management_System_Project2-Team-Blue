import { useEffect, useState } from "react";
import apiClient from "../api/client";
import ErrorAlert from "../components/ErrorAlert";
import Spinner from "../components/Spinner";
import { formatCurrency } from "../utils/quoteCalculations";

function AnalyticsDashboardPage() {
  const [analytics, setAnalytics] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      setError("");
      try {
        const response = await apiClient.get("/quotes/analytics");
        setAnalytics(response.data);
      } catch (err) {
        setError(err?.response?.data || "Failed to load analytics.");
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  if (loading) return <Spinner label="Loading dashboard..." />;

  return (
    <section className="stack">
      <h1>Analytics Dashboard</h1>
      <ErrorAlert message={error} />
      {analytics && (
        <div className="grid cards">
          <article className="card metric">
            <h2>Total Quotes</h2>
            <strong>{analytics.totalQuotes ?? 0}</strong>
          </article>
          <article className="card metric">
            <h2>Total Value</h2>
            <strong>{formatCurrency(analytics.totalValue)}</strong>
          </article>
          <article className="card metric">
            <h2>Average Value</h2>
            <strong>{formatCurrency(analytics.avgValue)}</strong>
          </article>
          <article className="card metric">
            <h2>Success Rate</h2>
            <strong>{analytics.successRate ?? 0}%</strong>
          </article>
        </div>
      )}
    </section>
  );
}

export default AnalyticsDashboardPage;
