import React from 'react';
import logo from './logo.svg';
import './App.css';
import MainLayout from './layouts/MainLayout';
import LoginRegisterWidget from "./components/LoginRegisterWidget";
import { AuthProvider } from './contexts/AuthContext';

function App() {
  return (
    <AuthProvider>
    <MainLayout>
      <h2>Welcome to Initiative!</h2>
      <LoginRegisterWidget />
    </MainLayout>
    </AuthProvider>
  );
}

export default App;
