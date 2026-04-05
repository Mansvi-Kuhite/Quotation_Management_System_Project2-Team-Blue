export const TAX_RATE = 0.18;

export function lineTotal(item) {
  const qty = Number(item.quantity || 0);
  const unitPrice = Number(item.unitPrice || 0);
  const discount = Number(item.discount || 0);
  return qty * unitPrice - discount;
}

export function computeQuoteTotals(items, discountAmount = 0) {
  const subTotal = items.reduce((sum, item) => sum + lineTotal(item), 0);
  const taxAmount = subTotal * TAX_RATE;
  const grandTotal = subTotal + taxAmount - Number(discountAmount || 0);

  return { subTotal, taxAmount, grandTotal };
}

export function formatCurrency(value) {
  return new Intl.NumberFormat("en-IN", {
    style: "currency",
    currency: "INR",
    maximumFractionDigits: 2
  }).format(Number(value || 0));
}
