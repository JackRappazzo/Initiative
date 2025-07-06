import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import MainLayout from "./layouts/MainLayout";
import LoginPage from "./pages/LoginPage";
import ListEncounters from "./pages/encounters/ListEncounters";
import EditEncounter from "./pages/encounters/EditEncounter";
import { AuthProvider } from './contexts/AuthContext';
import { UserProvider } from './contexts/UserContext';
import './App.css'; 

const App: React.FC = () => {
  return (
    <AuthProvider>
      <UserProvider>
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
      </UserProvider>
    </AuthProvider>
  );
};

export default App;
