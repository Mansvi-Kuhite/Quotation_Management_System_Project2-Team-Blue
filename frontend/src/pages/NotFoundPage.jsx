import { Link } from "react-router-dom";

function NotFoundPage() {
  return (
    <div className="center-screen">
      <div className="card">
        <h1>Page Not Found</h1>
        <p>The page you requested does not exist.</p>
        <Link className="btn btn-primary" to="/quotes">
          Go To Quotes
        </Link>
      </div>
    </div>
  );
}

export default NotFoundPage;
