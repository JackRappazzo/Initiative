import React from 'react';
import RegisterForm from '../components/auth/RegisterForm';
import '../App.css';

const RegisterPage: React.FC = () => {
  return (
    <div style={{ 
      display: 'flex', 
      justifyContent: 'center', 
      alignItems: 'center', 
      minHeight: '100vh',
      padding: '2rem'
    }}>
      <div style={{
        maxWidth: '400px',
        width: '100%',
        padding: '2rem',
        border: '1px solid #ddd',
        borderRadius: '8px',
        backgroundColor: 'white'
      }}>
        <RegisterForm />
      </div>
    </div>
  );
};

export default RegisterPage;
