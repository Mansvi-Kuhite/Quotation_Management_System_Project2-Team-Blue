import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

function Layout() {
  const { user, logout, isSalesRep, isManager, isAdmin } = useAuth();

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <h1>QMS</h1>
        <p className="user-meta">
          <strong>{user?.username || "Unknown"}</strong>
          <span>{user?.role || "No role"}</span>
        </p>
        <nav>
          <NavLink to="/quotes">Quotes</NavLink>
          {isSalesRep && <NavLink to="/quotes/create">Create Quote</NavLink>}
          {(isManager || isAdmin) && <NavLink to="/analytics">Analytics</NavLink>}
        </nav>
        <button className="btn btn-secondary" onClick={logout} type="button">
          Logout
        </button>
      </aside>
      <main className="content">
        <Outlet />
      </main>
    </div>
  );
}

export default Layout;
