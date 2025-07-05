import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import MainLayout from "./layouts/MainLayout";
import LoginPage from "./components/pages/LoginPage";
import EncountersPage from "./components/pages/Encounters";
import { AuthProvider } from './contexts/AuthContext';
import './App.css'; 

const App: React.FC = () => {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          {/* Login page without layout */}
          <Route path="/login" element={<LoginPage />} />
          {/* All other pages use MainLayout */}
          <Route element={<MainLayout />}>
            <Route path="/" element={<h2>Welcome to Initiative!</h2>} />
            <Route path="/encounters" element={<EncountersPage />} />
          </Route>
        </Routes>
      </Router>
    </AuthProvider>
  );
};

export default App;
