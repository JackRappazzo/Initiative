c# Azure Deployment Guide

This document explains how to properly deploy the Initiative React app to Azure to handle client-side routing.

## The Problem

Single Page Applications (SPAs) using React Router have client-side routing. When a user:
- Navigates using the app → Works fine ✅
- Refreshes the page or enters a URL directly → 404 Error ❌

This happens because the server tries to find a physical file for routes like `/encounters/123`, but only the React app knows about these routes.

## Solutions

### Azure Static Web Apps (Recommended)

If you're using Azure Static Web Apps, the `staticwebapp.config.json` file in the `public` folder will handle routing:

```json
{
  "routes": [
    {
      "route": "/static/*",
      "headers": {
        "Cache-Control": "public, max-age=31536000, immutable"
      }
    },
    {
      "route": "/*",
      "serve": "/index.html",
      "statusCode": 200
    }
  ],
  "navigationFallback": {
    "rewrite": "/index.html"
  }
}
```

### Azure App Service

If you're using Azure App Service, the `web.config` file will handle routing:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <rewrite>
      <rules>
        <rule name="React Routes" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
            <add input="{REQUEST_URI}" pattern="^/(api)" negate="true" />
          </conditions>
          <action type="Rewrite" url="/" />
        </rule>
      </rules>
    </rewrite>
  </system.webServer>
</configuration>
```

## Deployment Steps

1. **Build the app:**
   ```bash
   npm run build
   ```

2. **Deploy the `build` folder contents** to your Azure service

3. **Ensure the configuration file is included:**
   - For Static Web Apps: `staticwebapp.config.json` should be in the root of your deployed files
   - For App Service: `web.config` should be in the root of your deployed files

## Testing

After deployment, test these scenarios:
1. Navigate to your app root → Should work
2. Navigate to a specific route like `/encounters` → Should work
3. Refresh the page while on `/encounters` → Should work (not 404)
4. Directly enter a URL like `https://yourapp.com/encounters/123` → Should work

## Troubleshooting

### Still getting 404s?
- Check that the configuration file is in the deployed build folder
- Verify your Azure service type (Static Web Apps vs App Service)
- Check Azure logs for routing errors

### API calls failing?
- Verify the production environment variables are set correctly
- Check the API base URL doesn't have double slashes (`//`)
- Ensure CORS is configured on your API server

## Environment Variables

Make sure your production environment variables are correct:

```env
REACT_APP_API_BASE_URL=https://your-api.azurewebsites.net/api
REACT_APP_SIGNALR_HUB_URL=https://your-api.azurewebsites.net/lobby
```

Note: No double slashes in URLs!
