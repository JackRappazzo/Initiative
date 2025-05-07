import React from 'react';
import logo from './logo.svg';
import './App.css';
import MainLayout from './layouts/MainLayout';
import LoginRegisterWidget from "./components/LoginRegisterWidget";

function App() {
  return (
    <MainLayout>
      <h2>Welcome to Initiative!</h2>
      <LoginRegisterWidget />
    </MainLayout>
  );
}

export default App;
