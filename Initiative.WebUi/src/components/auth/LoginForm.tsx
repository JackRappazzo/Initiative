// src/components/LoginForm.tsx
import { useState, useContext, FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { AuthContext } from "../../contexts/AuthContext";
import { AdminClient } from "../../api/adminClient";

const LoginForm = () => {
  const navigate = useNavigate();
  const { login } = useContext(AuthContext);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleLogin = async (e: FormEvent) => {
    e.preventDefault();
    setError("");

    try {
      const adminClient = new AdminClient();
      const res = await adminClient.Login(email, password);
      if (res.success && res.jwt) {
        login(res.jwt);
        navigate("/");
      } else {
        setError("Login failed");
      }
    } catch (err) {
      console.error(err);
      setError("Invalid email or password");
    }
  };

  return (
    <form onSubmit={handleLogin} className="login-form">
      {error && <div className="error-message">{error}</div>}
      <div className="form-group">
        <input 
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          placeholder="email@address.com"
          required
        />
      </div>
      <div className="form-group">
        <input 
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          placeholder="password"
          required
        />
      </div>
      <button type="submit" className="btn-primary">
        Log In
      </button>
    </form>
  );
};

export default LoginForm;