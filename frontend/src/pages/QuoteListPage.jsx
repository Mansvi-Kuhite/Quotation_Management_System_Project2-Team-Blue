import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import apiClient from "../api/client";
import ErrorAlert from "../components/ErrorAlert";
import Spinner from "../components/Spinner";
import { useAuth } from "../context/AuthContext";
import { formatCurrency } from "../utils/quoteCalculations";

function QuoteListPage() {
  const navigate = useNavigate();
  const { isAdmin, isManager, isSalesRep } = useAuth();

  const [quotes, setQuotes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [actionError, setActionError] = useState("");

  const fetchQuotes = async () => {
    setLoading(true);
    setError("");
    try {
      const response = await apiClient.get("/quotes");
      setQuotes(response.data || []);
    } catch (err) {
      setError(
        err?.response?.data?.message ||
          err?.userHint ||
          err?.message ||
          "Failed to load quotes."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchQuotes();
  }, []);

  const handleDelete = async (quoteId) => {
    setActionError("");
    try {
      await apiClient.delete(`/quotes/${quoteId}`);
      await fetchQuotes();
    } catch (err) {
      setActionError(err?.response?.data || "Delete failed.");
    }
  };

  const handleApprove = async (quoteId) => {
    setActionError("");
    try {
      await apiClient.put(`/quotes/${quoteId}/status`, JSON.stringify("Accepted"), {
        headers: { "Content-Type": "application/json" }
      });
      await fetchQuotes();
    } catch (err) {
      setActionError(err?.response?.data?.detail || err?.response?.data?.title || err?.response?.data || "Approve failed.");
    }
  };

  if (loading) return <Spinner label="Loading quotes..." />;

  return (
    <section className="stack">
      <div className="row-between">
        <h1>Quote List</h1>
        {isSalesRep && (
          <button className="btn btn-primary" onClick={() => navigate("/quotes/create")} type="button">
            Create Quote
          </button>
        )}
      </div>

      <ErrorAlert message={error} />
      {error && (
        <button className="btn btn-secondary" type="button" onClick={fetchQuotes}>
          Retry
        </button>
      )}
      <ErrorAlert message={actionError} />

      <div className="card table-wrap">
        <table>
          <thead>
            <tr>
              <th>Quote #</th>
              <th>Customer</th>
              <th>Status</th>
              <th>Total</th>
              <th>Created By</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {!error && quotes.length === 0 && (
              <tr>
                <td colSpan="6">No quotes found.</td>
              </tr>
            )}
            {!error && quotes.map((quote) => {
              const canEdit = quote.status === "Draft";
              return (
                <tr key={quote.quoteId}>
                  <td>{quote.quoteNumber || `Q-${quote.quoteId}`}</td>
                  <td>{quote.customerId}</td>
                  <td>{quote.status}</td>
                  <td>{formatCurrency(quote.grandTotal)}</td>
                  <td>{quote.createdBy}</td>
                  <td className="actions">
                    <Link className="btn btn-secondary" to={`/quotes/${quote.quoteId}`}>
                      View
                    </Link>
                    <Link
                      className={`btn btn-secondary ${!canEdit ? "disabled" : ""}`}
                      to={canEdit ? `/quotes/${quote.quoteId}/edit` : "#"}
                      onClick={(e) => !canEdit && e.preventDefault()}
                    >
                      Edit
                    </Link>
                    {(isManager || isAdmin) && quote.status === "Sent" && (
                      <button className="btn btn-primary" type="button" onClick={() => handleApprove(quote.quoteId)}>
                        Approve
                      </button>
                    )}
                    {isAdmin && (
                      <button className="btn btn-danger" type="button" onClick={() => handleDelete(quote.quoteId)}>
                        Delete
                      </button>
                    )}
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </section>
  );
}

export default QuoteListPage;
