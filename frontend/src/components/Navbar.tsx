import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export function Navbar() {
  const { user, logout } = useAuth();

  return (
    <nav className="navbar">
      <Link to="/" className="navbar-brand">Recipe World</Link>
      <div className="navbar-links">
        <Link to="/">Home</Link>
        <Link to="/cuisines">Cuisines</Link>
        {user ? (
          <>
            <Link to="/my-recipes">My Recipes</Link>
            <Link to="/suggestions">Suggestions</Link>
            <span className="navbar-user">{user.displayName}</span>
            <button type="button" onClick={logout}>Logout</button>
          </>
        ) : (
          <>
            <Link to="/login">Login</Link>
            <Link to="/register">Register</Link>
          </>
        )}
      </div>
    </nav>
  );
}
