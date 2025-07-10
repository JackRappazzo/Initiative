import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import MainLayout from "./layouts/MainLayout";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import HomePage from "./pages/HomePage";
import ListEncounters from "./pages/encounters/ListEncounters";
import EditEncounter from "./pages/encounters/EditEncounter";
import { JoinLobby, Lobby } from "./pages/lobby";
import { AuthProvider } from './contexts/AuthContext';
import { UserProvider } from './contexts/UserContext';
import './App.css'; 

const App: React.FC = () => {
  return (
    <AuthProvider>
      <UserProvider>
        <Router>
          <Routes>
            {/* Login and Register pages without layout */}
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            {/* All other pages use MainLayout */}
            <Route element={<MainLayout />}>
              <Route path="/" element={<HomePage />} />
              <Route path="/encounters" element={<ListEncounters />} />
              <Route path="/encounters/:encounterId" element={<EditEncounter />} />
              <Route path="/lobby" element={<JoinLobby />} />
              <Route path="/lobby/:roomCode" element={<Lobby />} />
            </Route>
          </Routes>
        </Router>
      </UserProvider>
    </AuthProvider>
  );
};

export default App;
