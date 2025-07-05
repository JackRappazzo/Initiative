import React, { useContext, useState } from "react";
import { AuthContext } from "../contexts/AuthContext";

const SidebarMenu: React.FC = () => {
  const { isLoggedIn, logout } = useContext(AuthContext);
  const [open, setOpen] = useState(false);

  const handleLogout = () => {
    logout();
    setOpen(false);
  };

  return (
    <div className={`sidebar-menu ${open ? "open" : ""}`}>
      {/* Toggle Button */}
      <button
        className="sidebar-toggle-btn"
        onClick={() => setOpen((o) => !o)}
        aria-label={open ? "Close menu" : "Open menu"}
      >
        ☰
      </button>

      {/* Sidebar */}
      <nav>
        <ul>
          <li>
            <a href="/lobby" onClick={() => setOpen(false)}>Join</a>
          </li>
          {!isLoggedIn() && (
            <>
              <li>
                <a href="/login" onClick={() => setOpen(false)}>Sign In</a>
              </li>
              <li>
                <a href="/register" onClick={() => setOpen(false)}>Register</a>
              </li>
            </>
          )}
          {isLoggedIn() && (
            <>
              <li>
                <a href="/encounters" onClick={() => setOpen(false)}>Encounters</a>
              </li>
              <li>
                <button onClick={handleLogout}>Logout</button>
              </li>
            </>
          )}
        </ul>
      </nav>
    </div>
  );
};

export default SidebarMenu;