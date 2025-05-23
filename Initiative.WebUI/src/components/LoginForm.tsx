// src/components/LoginForm.tsx
import { useState, useContext, FormEvent } from "react";
import axios from "axios";
import { AuthContext } from "../contexts/AuthContext";
import { AdminClient } from "../api/adminClient";
import { LoginResponse } from "../api/messages/LoginResponse";
import { collapseTextChangeRangesAcrossMultipleVersions } from "typescript";



const LoginForm = () => {
  const { login } = useContext(AuthContext);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const adminClient = new AdminClient();

  const handleLogin = async (e: FormEvent) => {
    e.preventDefault();

    
    try {
      const res = await adminClient.Login(email,password);
      login(res.token);
    } catch (err) {
      console.error(err);
      alert("Login failed");
    }
  };

  return (
    <form onSubmit={handleLogin}>
      <input value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Email" />
      <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Password" />
      <button type="submit">Log In</button>
    </form>
  );
};

export default LoginForm;