// Environment configuration
export const config = {
  apiBaseUrl: process.env.REACT_APP_API_BASE_URL || 'https://localhost:7034/api',
  signalRHubUrl: process.env.REACT_APP_SIGNALR_HUB_URL || 'https://localhost:7034/lobby',
  
  // Add other environment variables here as needed
  // environment: process.env.NODE_ENV || 'development',
};

// Validate required environment variables
const requiredEnvVars = ['REACT_APP_API_BASE_URL', 'REACT_APP_SIGNALR_HUB_URL'];

export const validateEnvironment = () => {
  const missing = requiredEnvVars.filter(
    envVar => !process.env[envVar]
  );
  
  if (missing.length > 0) {
    console.warn(
      `Missing environment variables: ${missing.join(', ')}. ` +
      'Using default values. Create a .env.local file to customize.'
    );
  }
};

// Call validation on module load
validateEnvironment();
