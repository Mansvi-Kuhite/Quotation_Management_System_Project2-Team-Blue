import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import apiClient from "../api/client";
import ErrorAlert from "../components/ErrorAlert";
import QuoteForm from "../components/QuoteForm";
import Spinner from "../components/Spinner";

const formatDateInput = (value) => {
  if (!value) return "";
  const d = new Date(value);
  return Number.isNaN(d.getTime()) ? "" : d.toISOString().split("T")[0];
};

function mapItem(item) {
  return {
    id: item.id,
    productName: item.productName || "",
    quantity: item.quantity || 1,
    unitPrice: item.unitPrice || 0,
    discount: item.discount || 0
  };
}

function EditQuotePage() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");
  const [quote, setQuote] = useState(null);
  const [form, setForm] = useState({
    customerId: "",
    quoteDate: "",
    expiryDate: "",
    discountAmount: 0,
    lineItems: []
  });

  useEffect(() => {
    const loadQuote = async () => {
      setLoading(true);
      setError("");
      try {
        const response = await apiClient.get(`/quotes/${id}`);
        const data = response.data;
        setQuote(data);
        setForm({
          customerId: data.customerId,
          quoteDate: formatDateInput(data.quoteDate),
          expiryDate: formatDateInput(data.expiryDate),
          discountAmount: data.discountAmount || 0,
          lineItems: (data.lineItems || []).map(mapItem)
        });
      } catch (err) {
        setError(err?.response?.data || "Failed to load quote.");
      } finally {
        setLoading(false);
      }
    };
    loadQuote();
  }, [id]);

  const isEditable = useMemo(() => quote?.status === "Draft", [quote]);

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
    if (!isEditable) return;

    setBusy(true);
    setError("");
    try {
      const newItems = form.lineItems.filter((item) => !item.id && item.productName?.trim());
      for (const item of newItems) {
        await apiClient.post(`/quotes/${id}/items`, {
          productName: item.productName,
          quantity: Number(item.quantity),
          unitPrice: Number(item.unitPrice),
          discount: Number(item.discount || 0)
        });
      }
      navigate(`/quotes/${id}`);
    } catch (err) {
      setError(err?.response?.data || "Failed to save changes.");
    } finally {
      setBusy(false);
    }
  };

  if (loading) return <Spinner label="Loading quote..." />;

  return (
    <section className="stack">
      <div className="row-between">
        <h1>Edit Quote #{quote?.quoteNumber || quote?.quoteId}</h1>
      </div>
      {!isEditable && <ErrorAlert message="This quote is not in Draft status and cannot be edited." />}
      <ErrorAlert message={error} />
      <QuoteForm
        form={form}
        onFieldChange={onFieldChange}
        onLineItemChange={onLineItemChange}
        onAddLineItem={onAddLineItem}
        onRemoveLineItem={onRemoveLineItem}
        onSubmit={onSubmit}
        submitLabel="Save Changes"
        busy={busy}
        disabled={!isEditable}
        readOnlyDates
      />
      <p className="note">Current backend supports adding new items in edit flow; existing item updates are API-limited.</p>
    </section>
  );
}

export default EditQuotePage;
