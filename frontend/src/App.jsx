import { Navigate, Route, Routes } from "react-router-dom";
import Layout from "./components/Layout";
import ProtectedRoute from "./components/ProtectedRoute";
import AnalyticsDashboardPage from "./pages/AnalyticsDashboardPage";
import CreateQuotePage from "./pages/CreateQuotePage";
import EditQuotePage from "./pages/EditQuotePage";
import LoginPage from "./pages/LoginPage";
import NotFoundPage from "./pages/NotFoundPage";
import QuoteDetailsPage from "./pages/QuoteDetailsPage";
import QuoteListPage from "./pages/QuoteListPage";

function App() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/login" replace />} />
      <Route path="/login" element={<LoginPage />} />

      <Route
        element={
          <ProtectedRoute>
            <Layout />
          </ProtectedRoute>
        }
      >
        <Route path="/quotes" element={<QuoteListPage />} />
        <Route
          path="/quotes/create"
          element={
            <ProtectedRoute roles={["SalesRep"]}>
              <CreateQuotePage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/quotes/:id/edit"
          element={
            <ProtectedRoute roles={["SalesRep"]}>
              <EditQuotePage />
            </ProtectedRoute>
          }
        />
        <Route path="/quotes/:id" element={<QuoteDetailsPage />} />
        <Route
          path="/analytics"
          element={
            <ProtectedRoute roles={["SalesManager", "Admin"]}>
              <AnalyticsDashboardPage />
            </ProtectedRoute>
          }
        />
      </Route>

      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}

export default App;
