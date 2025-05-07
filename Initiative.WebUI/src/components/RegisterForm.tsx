// src/components/LoginForm.tsx
import { useState, useContext, FormEvent } from "react";
import axios from "axios";
import { AuthContext } from "../contexts/AuthContext";
import { AdminClient } from "../api/adminClient";
import { collapseTextChangeRangesAcrossMultipleVersions } from "typescript";



const RegisterForm = () => {
  const [displayName, setDisplayName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const adminClient = new AdminClient();

  const handleRegister = async (e: FormEvent) => {
    e.preventDefault();

    
    try {
      const res = await adminClient.Register(displayName, email,password);
    } catch (err) {
      console.error(err);
      alert("Registration failed");
    }
  };

  return (
    <form onSubmit={handleRegister}>
      <input value={displayName} onChange={(e) => setDisplayName(e.target.value)} placeholder="Display Name" />
      <input value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Email" />
      <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Password" />
      <button type="submit">Register</button>
    </form>
  );
};

export default RegisterForm;