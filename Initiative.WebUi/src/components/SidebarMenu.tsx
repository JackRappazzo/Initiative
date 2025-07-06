import React, { useContext, useState } from "react";
import { Link } from "react-router-dom";
import { AuthContext } from "../contexts/AuthContext";
import { useUser } from "../contexts/UserContext";
import "./SidebarMenu.css";

const SidebarMenu: React.FC = () => {
  const { isLoggedIn, logout } = useContext(AuthContext);
  const { userInfo } = useUser();
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
      {isLoggedIn() && userInfo && (
        <div className="room-code">
          Room Code: {userInfo.roomCode}
        </div>
      )}
      <nav>
        <ul>
          <li>
            <Link to="/lobby" onClick={() => setOpen(false)}>Join</Link>
          </li>
          {!isLoggedIn() && (
            <>
              <li>
                <Link to="/login" onClick={() => setOpen(false)}>Sign In</Link>
              </li>
              <li>
                <Link to="/register" onClick={() => setOpen(false)}>Register</Link>
              </li>
            </>
          )}
          {isLoggedIn() && (
            <>
              <li>
                <Link to="/encounters" onClick={() => setOpen(false)}>Encounters</Link>
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