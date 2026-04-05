function normalizeMessage(message) {
  if (!message) return "";
  if (typeof message === "string") return message;
  if (typeof message === "number" || typeof message === "boolean") return String(message);

  if (typeof message === "object") {
    return (
      message.message ||
      message.detail ||
      message.title ||
      "An unexpected error occurred."
    );
  }

  return "An unexpected error occurred.";
}

function ErrorAlert({ message }) {
  const text = normalizeMessage(message);
  if (!text) return null;

  return (
    <div className="alert alert-error" role="alert">
      {text}
    </div>
  );
}

export default ErrorAlert;
