import { computeQuoteTotals, formatCurrency, lineTotal } from "../utils/quoteCalculations";

const blankItem = { productName: "", quantity: 1, unitPrice: 0, discount: 0 };

function QuoteForm({
  form,
  onFieldChange,
  onLineItemChange,
  onAddLineItem,
  onRemoveLineItem,
  onSubmit,
  submitLabel,
  disabled,
  busy,
  readOnlyDates = false
}) {
  const totals = computeQuoteTotals(form.lineItems, form.discountAmount);

  return (
    <form onSubmit={onSubmit} className="stack">
      <section className="card">
        <h2>Quote Basics</h2>
        <div className="grid two">
          <label>
            Customer ID
            <input
              type="number"
              min="1"
              value={form.customerId}
              onChange={(e) => onFieldChange("customerId", e.target.value)}
              disabled={disabled}
              required
            />
          </label>
          <label>
            Quote Date
            <input
              type="date"
              value={form.quoteDate || ""}
              onChange={(e) => onFieldChange("quoteDate", e.target.value)}
              disabled={disabled || readOnlyDates}
              required
            />
          </label>
          <label>
            Expiry Date
            <input
              type="date"
              value={form.expiryDate || ""}
              min={form.quoteDate || undefined}
              onChange={(e) => onFieldChange("expiryDate", e.target.value)}
              disabled={disabled || readOnlyDates}
              required
            />
          </label>
          <label>
            Header Discount
            <input
              type="number"
              min="0"
              step="0.01"
              value={form.discountAmount}
              onChange={(e) => onFieldChange("discountAmount", e.target.value)}
              disabled={disabled}
            />
          </label>
        </div>
      </section>

      <section className="card">
        <div className="row-between">
          <h2>Line Items</h2>
          <button
            type="button"
            className="btn btn-secondary"
            onClick={() => onAddLineItem(blankItem)}
            disabled={disabled}
          >
            Add Item
          </button>
        </div>
        {form.lineItems.length === 0 && <p>No line items yet.</p>}
        {form.lineItems.map((item, index) => (
          <div key={item.tempId || item.id || index} className="line-item-grid">
            <label>
              Product
              <input
                value={item.productName}
                onChange={(e) => onLineItemChange(index, "productName", e.target.value)}
                disabled={disabled}
                required
              />
            </label>
            <label>
              Qty
              <input
                type="number"
                min="1"
                value={item.quantity}
                onChange={(e) => onLineItemChange(index, "quantity", e.target.value)}
                disabled={disabled}
                required
              />
            </label>
            <label>
              Unit Price
              <input
                type="number"
                min="0"
                step="0.01"
                value={item.unitPrice}
                onChange={(e) => onLineItemChange(index, "unitPrice", e.target.value)}
                disabled={disabled}
                required
              />
            </label>
            <label>
              Discount
              <input
                type="number"
                min="0"
                step="0.01"
                value={item.discount}
                onChange={(e) => onLineItemChange(index, "discount", e.target.value)}
                disabled={disabled}
              />
            </label>
            <div className="line-total">
              <span>Total</span>
              <strong>{formatCurrency(lineTotal(item))}</strong>
            </div>
            <button
              type="button"
              className="btn btn-danger"
              onClick={() => onRemoveLineItem(index)}
              disabled={disabled}
            >
              Remove
            </button>
          </div>
        ))}
      </section>

      <section className="card">
        <h2>Computed Totals</h2>
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
            <span>Grand Total</span>
            <strong>{formatCurrency(totals.grandTotal)}</strong>
          </p>
        </div>
      </section>

      <button type="submit" className="btn btn-primary" disabled={disabled || busy}>
        {busy ? "Saving..." : submitLabel}
      </button>
    </form>
  );
}

export default QuoteForm;
