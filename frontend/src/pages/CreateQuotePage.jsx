import { useState } from "react";
import { useNavigate } from "react-router-dom";
import apiClient from "../api/client";
import ErrorAlert from "../components/ErrorAlert";
import QuoteForm from "../components/QuoteForm";

const formatDateInput = (date) => new Date(date).toISOString().split("T")[0];
const today = new Date();
const defaultExpiry = new Date(today);
defaultExpiry.setDate(defaultExpiry.getDate() + 30);

const initialForm = {
  customerId: "",
  quoteDate: formatDateInput(today),
  expiryDate: formatDateInput(defaultExpiry),
  discountAmount: 0,
  lineItems: [{ tempId: crypto.randomUUID(), productName: "", quantity: 1, unitPrice: 0, discount: 0 }]
};

function CreateQuotePage() {
  const navigate = useNavigate();
  const [form, setForm] = useState(initialForm);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");

  const onFieldChange = (field, value) => {
    setForm((prev) => ({ ...prev, [field]: value }));
  };

  const onLineItemChange = (index, field, value) => {
    setForm((prev) => {
      const next = [...prev.lineItems];
      next[index] = { ...next[index], [field]: value };
      return { ...prev, lineItems: next };
    });
  };

  const onAddLineItem = (item) => {
    setForm((prev) => ({
      ...prev,
      lineItems: [...prev.lineItems, { ...item, tempId: crypto.randomUUID() }]
    }));
  };

  const onRemoveLineItem = (index) => {
    setForm((prev) => ({
      ...prev,
      lineItems: prev.lineItems.filter((_, i) => i !== index)
    }));
  };

  const onSubmit = async (e) => {
    e.preventDefault();
    setBusy(true);
    setError("");

    try {
      if (!form.quoteDate || !form.expiryDate) {
        throw new Error("Quote date and expiry date are required.");
      }
      if (new Date(form.expiryDate) < new Date(form.quoteDate)) {
        throw new Error("Expiry date must be greater than or equal to quote date.");
      }

      const quotePayload = {
        customerId: Number(form.customerId),
        quoteDate: new Date(`${form.quoteDate}T00:00:00`).toISOString(),
        expiryDate: new Date(`${form.expiryDate}T00:00:00`).toISOString(),
        discountAmount: Number(form.discountAmount || 0)
      };

      const quoteResponse = await apiClient.post("/quotes", quotePayload);
      const quoteId = quoteResponse.data?.quoteId;
      if (!quoteId) {
        throw new Error("Quote ID not returned from create response.");
      }

      const validItems = form.lineItems.filter((x) => x.productName?.trim());
      for (const item of validItems) {
        await apiClient.post(`/quotes/${quoteId}/items`, {
          productName: item.productName,
          quantity: Number(item.quantity),
          unitPrice: Number(item.unitPrice),
          discount: Number(item.discount || 0)
        });
      }

      navigate(`/quotes/${quoteId}`);
    } catch (err) {
      const apiData = err?.response?.data;
      const message =
        (!err?.response && "Cannot reach API. Start backend and verify URL/port.") ||
        (!err?.response && err?.userHint) ||
        (typeof apiData === "string" && apiData) ||
        apiData?.message ||
        apiData?.detail ||
        apiData?.title ||
        err.message ||
        "Failed to create quote.";
      setError(message);
    } finally {
      setBusy(false);
    }
  };

  return (
    <section className="stack">
      <h1>Create Quote</h1>
      <ErrorAlert message={error} />
      <QuoteForm
        form={form}
        onFieldChange={onFieldChange}
        onLineItemChange={onLineItemChange}
        onAddLineItem={onAddLineItem}
        onRemoveLineItem={onRemoveLineItem}
        onSubmit={onSubmit}
        submitLabel="Create Quote"
        busy={busy}
        disabled={false}
      />
    </section>
  );
}

export default CreateQuotePage;
