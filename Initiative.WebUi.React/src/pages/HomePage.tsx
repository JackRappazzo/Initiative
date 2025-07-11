import React, { useContext } from 'react';
import { Link } from 'react-router-dom';
import { AuthContext } from '../contexts/AuthContext';
import ListEncounters from './encounters/ListEncounters';
import './HomePage.css';

const HomePage: React.FC = () => {
  const { isLoggedIn } = useContext(AuthContext);

  if (isLoggedIn()) {
    // If user is logged in, show the encounters list
    return <ListEncounters />;
  }

  // If user is not logged in, show welcome message with options
  return (
    <div className="homepage-container">
      <div className="homepage-content">
        <h1>Welcome to Initiative!</h1>
        
        <div className="homepage-options">
          <div className="option-card">
            <h2>Dungeon Master?</h2>
            <p>If you're a dungeon master, register here!</p>
            <Link to="/register" className="btn-primary">
              Register
            </Link>
          </div>
          
          <div className="option-card">
            <h2>Have a room code?</h2>
            <p>Join your session here</p>
            <Link to="/lobby" className="btn-secondary">
              Join Session
            </Link>
          </div>
          
          <div className="option-card">
            <h2>Already have an account?</h2>
            <p>Sign in to manage your encounters</p>
            <Link to="/login" className="btn-tertiary">
              Sign In
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default HomePage;
