import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import ErrorAlert from "../components/ErrorAlert";
import Spinner from "../components/Spinner";
import { useAuth } from "../context/AuthContext";

function LoginPage() {
  const { isAuthenticated, login, user } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");

  const redirectTo = location.state?.from || "/quotes";

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setBusy(true);
    try {
      await login(username, password);
      navigate(redirectTo, { replace: true });
    } catch (err) {
      let message = "Login failed.";
      if (!err?.response) {
        message =
          "Cannot reach API. Start backend and verify it is running on http://localhost:5001.";
      } else if (err.response.status === 401) {
        message = "Invalid credentials.";
      } else if (err.response.status === 404) {
        message = "Login endpoint not found. Check API base URL/proxy configuration.";
      } else if (typeof err.response.data === "string" && err.response.data.trim()) {
        message = `Login failed (${err.response.status}): ${err.response.data}`;
      } else if (err.response.data && typeof err.response.data === "object") {
        const apiMsg =
          err.response.data.message ||
          err.response.data.title ||
          err.response.data.detail ||
          "";
        message = apiMsg
          ? `Login failed (${err.response.status}): ${apiMsg}`
          : `Login failed (${err.response.status}).`;
      } else {
        message = `Login failed (${err.response.status}).`;
      }
      if (!err?.response && err?.userHint) {
        message = err.userHint;
      }
      setError(message);
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="center-screen">
      <form className="card login-card" onSubmit={handleSubmit}>
        <h1>Quotation Management</h1>
        <p>Sign in to continue.</p>
        {isAuthenticated && (
          <p className="note">
            Signed in as <strong>{user?.username}</strong>. Sign in again to switch account.
          </p>
        )}

        <label>
          Username
          <input value={username} onChange={(e) => setUsername(e.target.value)} required />
        </label>
        <label>
          Password
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </label>

        <ErrorAlert message={error} />
        {busy ? (
          <Spinner label="Signing in..." />
        ) : (
          <button type="submit" className="btn btn-primary">
            Login
          </button>
        )}
      </form>
    </div>
  );
}

export default LoginPage;
