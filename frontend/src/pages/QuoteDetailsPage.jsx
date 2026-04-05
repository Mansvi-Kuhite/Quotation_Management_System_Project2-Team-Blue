import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import apiClient from "../api/client";
import ErrorAlert from "../components/ErrorAlert";
import Spinner from "../components/Spinner";
import { useAuth } from "../context/AuthContext";
import { computeQuoteTotals, formatCurrency } from "../utils/quoteCalculations";

function QuoteDetailsPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { isSalesRep, isManager, isAdmin } = useAuth();

  const [quote, setQuote] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [actionError, setActionError] = useState("");

  const fetchQuote = async () => {
    setLoading(true);
    setError("");
    try {
      const response = await apiClient.get(`/quotes/${id}`);
      setQuote(response.data);
    } catch (err) {
      setError(err?.response?.data || "Failed to load quote.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchQuote();
  }, [id]);

  const handleSetStatus = async (status) => {
    setActionError("");
    try {
      await apiClient.put(`/quotes/${id}/status`, JSON.stringify(status), {
        headers: { "Content-Type": "application/json" }
      });
      await fetchQuote();
    } catch (err) {
      const apiData = err?.response?.data;
      if (err?.response?.status === 403) {
        setActionError("You are not authorized to change quote status.");
      } else if (typeof apiData === "string") {
        setActionError(apiData);
      } else {
        setActionError(apiData?.detail || apiData?.title || "Status update failed.");
      }
    }
  };

  const handleDelete = async () => {
    setActionError("");
    try {
      await apiClient.delete(`/quotes/${id}`);
      navigate("/quotes");
    } catch (err) {
      setActionError(err?.response?.data || "Delete failed.");
    }
  };

  if (loading) return <Spinner label="Loading quote details..." />;
  if (!quote) return <ErrorAlert message="Quote not found." />;

  const totals = computeQuoteTotals(quote.lineItems || [], quote.discountAmount || 0);
  const canEdit = quote.status === "Draft";
  const canSend = (isManager || isAdmin) && quote.status === "Draft";
  const canApprove = (isManager || isAdmin) && quote.status === "Sent";

  return (
    <section className="stack">
      <div className="row-between">
        <h1>Quote Details</h1>
        <div className="actions">
          <Link className={`btn btn-secondary ${!canEdit ? "disabled" : ""}`} to={canEdit ? `/quotes/${id}/edit` : "#"}>
            Edit
          </Link>
          {canSend && (
            <button className="btn btn-primary" type="button" onClick={() => handleSetStatus("Sent")}>
              Mark Sent
            </button>
          )}
          {canApprove && (
            <button className="btn btn-primary" type="button" onClick={() => handleSetStatus("Accepted")}>
              Approve
            </button>
          )}
          {isAdmin && (
            <button className="btn btn-danger" type="button" onClick={handleDelete}>
              Delete
            </button>
          )}
        </div>
      </div>
      {isSalesRep && quote.status === "Draft" && (
        <p className="note">Draft quotes can be sent by SalesManager/Admin as per current role rules.</p>
      )}

      <ErrorAlert message={error} />
      <ErrorAlert message={actionError} />

      <section className="card">
        <div className="grid two">
          <p>
            <strong>Quote ID:</strong> {quote.quoteId}
          </p>
          <p>
            <strong>Quote Number:</strong> {quote.quoteNumber || `Q-${quote.quoteId}`}
          </p>
          <p>
            <strong>Customer ID:</strong> {quote.customerId}
          </p>
          <p>
            <strong>Status:</strong> {quote.status}
          </p>
          <p>
            <strong>Created By:</strong> {quote.createdBy}
          </p>
          <p>
            <strong>Created Date:</strong> {new Date(quote.createdDate).toLocaleString()}
          </p>
        </div>
      </section>

      <section className="card table-wrap">
        <h2>Line Items</h2>
        <table>
          <thead>
            <tr>
              <th>Product</th>
              <th>Quantity</th>
              <th>Unit Price</th>
              <th>Discount</th>
              <th>Line Total</th>
            </tr>
          </thead>
          <tbody>
            {(quote.lineItems || []).map((item) => (
              <tr key={item.id}>
                <td>{item.productName}</td>
                <td>{item.quantity}</td>
                <td>{formatCurrency(item.unitPrice)}</td>
                <td>{formatCurrency(item.discount)}</td>
                <td>{formatCurrency(item.lineTotal || item.quantity * item.unitPrice - item.discount)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </section>

      <section className="card">
        <h2>Totals</h2>
        <div className="totals">
          <p>
            <span>Sub Total</span>
            <strong>{formatCurrency(totals.subTotal)}</strong>
          </p>
          <p>
            <span>Tax (18%)</span>
            <strong>{formatCurrency(totals.taxAmount)}</strong>
          </p>
          <p>
            <span>Header Discount</span>
            <strong>{formatCurrency(quote.discountAmount)}</strong>
          </p>
          <p>
            <span>Grand Total</span>
            <strong>{formatCurrency(totals.grandTotal)}</strong>
          </p>
        </div>
      </section>
    </section>
  );
}

export default QuoteDetailsPage;
