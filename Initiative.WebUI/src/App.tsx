import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import MainLayout from "./layouts/MainLayout";
import LoginPage from "./components/pages/LoginPage";
import ListEncounters from "./components/pages/encounters/ListEncounters";
import EditEncounter from "./components/pages/encounters/EditEncounter";
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
            <Route path="/encounters" element={<ListEncounters />} />
            <Route path="/encounters/:encounterId" element={<EditEncounter />} />
          </Route>
        </Routes>
      </Router>
    </AuthProvider>
  );
};

export default App;
