import { useState, FormEvent } from "react";
import { AdminClient } from "../../api/adminClient";
import "./RegisterForm.css";



const RegisterForm = () => {
  const [displayName, setDisplayName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);
  const adminClient = new AdminClient();

  const handleRegister = async (e: FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);

    try {
      await adminClient.Register(displayName, email, password);
      setSuccess(true);
      setDisplayName("");
      setEmail("");
      setPassword("");
    } catch (err) {
      console.error(err);
      setError("Registration failed. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  if (success) {
    return (
      <div className="success-message">
        <h2>Registration Successful!</h2>
        <p>Please sign in with your new account.</p>
      </div>
    );
  }

  return (
    <div className="register-form-container">
      <h2>Register</h2>
      {error && <div className="error-message">{error}</div>}
      <form className="register-form" onSubmit={handleRegister}>
        <input 
          value={displayName} 
          onChange={(e) => setDisplayName(e.target.value)} 
          placeholder="Display Name" 
          required
          disabled={isLoading}
        />
        <input 
          type="email"
          value={email} 
          onChange={(e) => setEmail(e.target.value)} 
          placeholder="Email" 
          required
          disabled={isLoading}
        />
        <input 
          type="password" 
          value={password} 
          onChange={(e) => setPassword(e.target.value)} 
          placeholder="Password" 
          required
          disabled={isLoading}
        />
        <button type="submit" disabled={isLoading}>
          {isLoading ? 'Registering...' : 'Register'}
        </button>
      </form>
    </div>
  );
};

export default RegisterForm;