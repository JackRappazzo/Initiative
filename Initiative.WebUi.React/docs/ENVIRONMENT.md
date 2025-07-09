# Environment Variables Setup

This project uses environment variables to configure API endpoints and other settings.

## Setup

1. **Copy the example file:**
   ```bash
   cp .env.example .env.local
   ```

2. **Edit `.env.local`** with your specific configuration:
   ```env
   REACT_APP_API_BASE_URL=https://your-api-server.com/api
   REACT_APP_SIGNALR_HUB_URL=https://your-api-server.com/lobby
   ```

## Available Environment Variables

| Variable | Description | Default Value |
|----------|-------------|---------------|
| `REACT_APP_API_BASE_URL` | Base URL for the REST API | `https://localhost:7034/api` |
| `REACT_APP_SIGNALR_HUB_URL` | URL for the SignalR hub | `https://localhost:7034/lobby` |

## Environment Files

- `.env` - Default values (committed to git)
- `.env.development` - Development-specific values (committed to git)
- `.env.production` - Production values template (committed to git)
- `.env.local` - Local overrides (ignored by git - create from `.env.example`)

## Notes

- All React environment variables must start with `REACT_APP_`
- Environment variables are validated on application startup
- Missing variables will use defaults with a console warning
- For production deployment, set these variables in your hosting platform

## Development

For local development, the default values should work with a locally running API server on `https://localhost:7034`.

If you need to override any values for your local setup, create `.env.local` and only include the variables you want to override.
