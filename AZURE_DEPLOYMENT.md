# Azure Deployment Guide for RecipeCalculator Blazor WASM

## Overview

Since RecipeCalculator is a **Blazor WebAssembly (WASM)** application that runs entirely in the browser, it's just **static files** (HTML, CSS, JavaScript, and .NET WASM runtime). This makes deployment extremely simple and cost-effective on Azure.

## Recommended Approach: Azure Static Web Apps

**Azure Static Web Apps** is the BEST option for Blazor WASM because:

✅ **Built specifically for static sites** (React, Vue, Angular, Blazor WASM)  
✅ **FREE tier available** (100GB bandwidth/month, custom domains, SSL)  
✅ **Global CDN** included automatically  
✅ **Automatic HTTPS** with SSL certificates  
✅ **GitHub Actions CI/CD** built-in (auto-deploy on push)  
✅ **Custom domains** with automatic SSL  
✅ **Staging environments** for pull requests  
✅ **Zero infrastructure management**  
✅ **Perfect for apps with no backend** (all logic in browser)  

### Cost Comparison

| Service | Free Tier | Best For | Monthly Cost (if paid) |
|---------|-----------|----------|------------------------|
| **Static Web Apps** | ✅ YES (100GB bandwidth) | Blazor WASM (BEST) | $0 - $9 |
| App Service | ❌ NO | Apps with backends | $13+ |
| Storage + CDN | ⚠️ Limited | DIY approach | $5+ |
| Azure Functions | ✅ YES (1M requests) | API + Static files | $0+ |

## Deployment Options

### Option 1: Azure Static Web Apps (RECOMMENDED) ⭐

#### Why This is Best:
- **Purpose-built for Blazor WASM**
- **Free forever** (unless you exceed 100GB bandwidth)
- **No server to manage**
- **Automatic scaling**
- **Built-in CI/CD**

#### Setup Steps:

**Method A: Via Azure Portal (5 minutes)**

1. **Build your app locally:**
   ```bash
   cd /home/victor/Workspace/RecipeCalculator/src/RecipeCalculator.UI
   dotnet publish -c Release -o publish
   ```

2. **Go to Azure Portal:**
   - Navigate to: https://portal.azure.com
   - Click "+ Create a resource"
   - Search for "Static Web Apps"
   - Click "Create"

3. **Configure:**
   - **Subscription:** Select your subscription
   - **Resource Group:** Create new or select existing
   - **Name:** `recipecalculator` (will be: recipecalculator.azurestaticapps.net)
   - **Plan type:** Free
   - **Region:** Choose closest to your users
   - **Deployment source:** GitHub (or upload manually)

4. **Connect GitHub (Automatic CI/CD):**
   - Authorize Azure to access your GitHub
   - Select repository: `YourGitHub/RecipeCalculator`
   - Select branch: `main`
   - Build preset: `Blazor`
   - App location: `/src/RecipeCalculator.UI`
   - Output location: `wwwroot`

5. **Deploy:**
   - Click "Review + Create"
   - Azure creates a GitHub Action workflow automatically
   - Every push to `main` triggers auto-deployment

**Method B: Via Azure CLI (2 minutes)**

```bash
# Install Azure CLI Static Web Apps extension
az extension add --name staticwebapp

# Login
az login

# Create resource group
az group create --name RecipeCalculatorRG --location eastus

# Create static web app with GitHub integration
az staticwebapp create \
  --name recipecalculator \
  --resource-group RecipeCalculatorRG \
  --source https://github.com/YourUsername/RecipeCalculator \
  --branch main \
  --app-location "/src/RecipeCalculator.UI" \
  --output-location "wwwroot" \
  --login-with-github
```

**Method C: Manual Upload (No GitHub)**

```bash
# Install SWA CLI
npm install -g @azure/static-web-apps-cli

# Build
cd /home/victor/Workspace/RecipeCalculator/src/RecipeCalculator.UI
dotnet publish -c Release -o publish

# Deploy
swa deploy ./publish/wwwroot \
  --deployment-token <YOUR_DEPLOYMENT_TOKEN> \
  --env production
```

#### GitHub Action Workflow (Auto-created)

Azure creates `.github/workflows/azure-static-web-apps.yml`:

```yaml
name: Azure Static Web Apps CI/CD

on:
  push:
    branches:
      - main
  pull_request:
    types: [opened, synchronize, reopened, closed]
    branches:
      - main

jobs:
  build_and_deploy_job:
    runs-on: ubuntu-latest
    name: Build and Deploy Job
    steps:
      - uses: actions/checkout@v3
        with:
          submodules: true

      - name: Build And Deploy
        id: builddeploy
        uses: Azure/static-web-apps-deploy@v1
        with:
          azure_static_web_apps_api_token: ${{ secrets.AZURE_STATIC_WEB_APPS_API_TOKEN }}
          repo_token: ${{ secrets.GITHUB_TOKEN }}
          action: "upload"
          app_location: "/src/RecipeCalculator.UI"
          output_location: "wwwroot"
```

---

### Option 2: Azure Storage + CDN (Manual Control)

#### When to Use:
- Need more control over caching
- Want to use existing storage account
- Building custom DevOps pipeline

#### Cost: ~$1-5/month

#### Setup Steps:

```bash
# 1. Create storage account
az storage account create \
  --name recipecalcstorage \
  --resource-group RecipeCalculatorRG \
  --location eastus \
  --sku Standard_LRS \
  --kind StorageV2

# 2. Enable static website hosting
az storage blob service-properties update \
  --account-name recipecalcstorage \
  --static-website \
  --index-document index.html \
  --404-document index.html

# 3. Build and publish
cd /home/victor/Workspace/RecipeCalculator/src/RecipeCalculator.UI
dotnet publish -c Release -o publish

# 4. Upload to storage
az storage blob upload-batch \
  --source ./publish/wwwroot \
  --destination '$web' \
  --account-name recipecalcstorage

# 5. Get the URL
az storage account show \
  --name recipecalcstorage \
  --query "primaryEndpoints.web" \
  --output tsv
```

#### Add CDN (Optional but Recommended):

```bash
# Create CDN profile
az cdn profile create \
  --name RecipeCalculatorCDN \
  --resource-group RecipeCalculatorRG \
  --sku Standard_Microsoft

# Create CDN endpoint
az cdn endpoint create \
  --name recipecalculator \
  --profile-name RecipeCalculatorCDN \
  --resource-group RecipeCalculatorRG \
  --origin recipecalcstorage.z13.web.core.windows.net \
  --origin-host-header recipecalcstorage.z13.web.core.windows.net
```

---

### Option 3: Azure App Service (NOT RECOMMENDED for pure WASM)

#### When to Use:
- You add a backend API later (ASP.NET Core)
- Need server-side processing

#### Cost: Starts at $13/month (no free tier)

**Only use if you add server-side features!** For pure Blazor WASM, this is overkill and expensive.

---

## Configuration for Production

### 1. Update `wwwroot/index.html` Base Path

If deploying to a subdirectory, update:

```html
<!-- For root deployment (Static Web Apps) -->
<base href="/" />

<!-- For subdirectory deployment -->
<base href="/calculator/" />
```

### 2. Configure staticwebapp.config.json

Create `/src/RecipeCalculator.UI/wwwroot/staticwebapp.config.json`:

```json
{
  "navigationFallback": {
    "rewrite": "/index.html",
    "exclude": ["/images/*.{png,jpg,gif}", "/css/*"]
  },
  "mimeTypes": {
    ".json": "application/json",
    ".wasm": "application/wasm"
  },
  "globalHeaders": {
    "cache-control": "public, max-age=31536000, immutable"
  },
  "routes": [
    {
      "route": "/index.html",
      "headers": {
        "cache-control": "no-cache"
      }
    }
  ]
}
```

### 3. Environment-Specific Settings

For different LLM configurations per environment:

```csharp
// Add to Program.cs
#if DEBUG
    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
    {
        ["LLM:DefaultProvider"] = "Ollama",
        ["LLM:DefaultApiUrl"] = "http://localhost:11434"
    });
#endif
```

---

## Custom Domain Setup

### For Static Web Apps:

```bash
# Add custom domain
az staticwebapp hostname set \
  --name recipecalculator \
  --resource-group RecipeCalculatorRG \
  --hostname calculator.yourdomain.com

# SSL is automatic and free!
```

**DNS Configuration:**
```
Type: CNAME
Name: calculator
Value: recipecalculator.azurestaticapps.net
TTL: 3600
```

---

## Performance Optimization

### 1. Enable Compression (Automatic in Static Web Apps)

### 2. Optimize Build Size

Add to `RecipeCalculator.UI.csproj`:

```xml
<PropertyGroup>
  <BlazorWebAssemblyLoadAllGlobalizationData>false</BlazorWebAssemblyLoadAllGlobalizationData>
  <InvariantGlobalization>true</InvariantGlobalization>
  <RunAOTCompilation>true</RunAOTCompilation> <!-- Smaller, faster -->
</PropertyGroup>
```

### 3. Lazy Load Assemblies

For larger apps:

```xml
<ItemGroup>
  <BlazorWebAssemblyLazyLoad Include="RecipeCalculator.Engine.dll" />
</ItemGroup>
```

---

## Monitoring and Analytics

### Application Insights (Free Tier)

Add to `Program.cs`:

```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

Add to `wwwroot/index.html`:

```html
<script type="text/javascript">
  var appInsights = window.appInsights || function(config) {
    // Application Insights snippet
  }({
    connectionString: "YOUR_CONNECTION_STRING"
  });
</script>
```

---

## CI/CD Pipeline Examples

### GitHub Actions (Detailed)

`.github/workflows/deploy.yml`:

```yaml
name: Deploy to Azure Static Web Apps

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Run tests
        run: dotnet test --no-restore

      - name: Publish
        run: dotnet publish src/RecipeCalculator.UI/RecipeCalculator.UI.csproj -c Release -o publish

      - name: Deploy to Azure Static Web Apps
        uses: Azure/static-web-apps-deploy@v1
        with:
          azure_static_web_apps_api_token: ${{ secrets.AZURE_STATIC_WEB_APPS_API_TOKEN }}
          action: "upload"
          app_location: "publish/wwwroot"
          skip_app_build: true
```

---

## Cost Estimation

### Static Web Apps (Free Tier):
- **Bandwidth:** 100 GB/month FREE
- **Custom domains:** Unlimited FREE
- **SSL certificates:** FREE
- **Staging environments:** 3 FREE
- **Cost for typical usage:** **$0/month** 🎉

### If You Exceed Free Tier:
- Standard plan: $9/month
- Additional bandwidth: $0.15/GB

### Example Usage:
- **Small app:** 5MB download size
- **Users:** 1,000 visitors/month
- **Bandwidth:** 5MB × 1,000 = 5GB
- **Cost:** FREE (well within 100GB)

---

## Quick Start Commands

### Deploy to Azure Static Web Apps (Complete):

```bash
# 1. Install tools
az extension add --name staticwebapp
npm install -g @azure/static-web-apps-cli

# 2. Login
az login

# 3. Create and deploy
az staticwebapp create \
  --name recipecalculator \
  --resource-group RecipeCalculatorRG \
  --source https://github.com/YourUsername/RecipeCalculator \
  --location "East US 2" \
  --branch main \
  --app-location "/src/RecipeCalculator.UI" \
  --output-location "wwwroot" \
  --login-with-github

# 4. Done! Your app is live at:
# https://recipecalculator.azurestaticapps.net
```

---

## Important Notes for LLM Integration

⚠️ **Browser CORS Limitations:**

Since Blazor WASM runs in the browser:
- **Ollama (localhost)** won't work in production (CORS + localhost not accessible)
- **OpenAI/Anthropic** work fine (they support CORS)

**Solutions:**
1. **Use OpenAI/Anthropic APIs directly** (works in browser)
2. **Add an Azure Function** as a proxy for Ollama
3. **Deploy Ollama separately** with CORS enabled (e.g., Azure Container Instance)

**Option: Add Azure Functions API** (Recommended for Ollama support)

```bash
# Enable Azure Functions API in Static Web App
# Create: api/LlmProxy.cs

[FunctionName("LlmProxy")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
{
    // Forward request to Ollama/other LLM
    // Handles CORS automatically
}
```

---

## Conclusion

**Best Choice: Azure Static Web Apps**

For RecipeCalculator, a pure Blazor WASM application:

✅ **FREE hosting** (100GB bandwidth)  
✅ **5-minute setup**  
✅ **Automatic CI/CD** via GitHub  
✅ **Global CDN**  
✅ **Custom domains + SSL FREE**  
✅ **Zero server maintenance**  
✅ **Perfect for static sites**  

**One command deployment:**
```bash
az staticwebapp create --name recipecalculator \
  --resource-group RecipeCalculatorRG \
  --source https://github.com/YourUsername/RecipeCalculator \
  --branch main --app-location "/src/RecipeCalculator.UI" \
  --output-location "wwwroot" --login-with-github
```

**URL:** `https://recipecalculator.azurestaticapps.net`

**Cost:** $0/month (unless you get >100GB traffic)

🚀 **Deploy in 5 minutes, FREE forever!**
