import React, { useState, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { AdminClient } from "../../api/adminClient";
import { AuthContext } from "../../contexts/AuthContext";

const LoginPage: React.FC = () => {
  const navigate = useNavigate();
  const { login } = useContext(AuthContext);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const adminClient = new AdminClient();
      const response = await adminClient.Login(email, password);
      if (response.success && response.jwt) {
        login(response.jwt);
        navigate("/");
      } else {
        setError("Invalid credentials.");
      }
    } catch (err) {
      setError("Login failed. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="login-form">
      <h1>Login</h1>
      {error && <div className="error-message">{error}</div>}
      <div className="form-group">
        <input
          type="email"
          value={email}
          onChange={e => setEmail(e.target.value)}
          placeholder="email@address.com"
          required
        />
      </div>
      <div className="form-group">
        <input
          type="password"
          value={password}
          onChange={e => setPassword(e.target.value)}
          placeholder="password"
          required
        />
      </div>
      <button type="submit" className="btn-primary" disabled={loading}>
        {loading ? "Logging in..." : "Log In"}
      </button>
    </form>
  );
};

export default LoginPage;