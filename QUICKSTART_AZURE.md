# Quick Start: Deploy to Azure in 5 Minutes ⚡

## Prerequisites

- Azure account (free tier works!) - [Sign up here](https://azure.microsoft.com/free/)
- Azure CLI installed - [Install guide](https://docs.microsoft.com/cli/azure/install-azure-cli)
- GitHub account (optional, for auto-deploy)

## Option 1: Automated Script (Easiest) 🚀

```bash
# Run the deployment script
./deploy-azure.sh
```

Follow the prompts:
1. Enter your Azure details (or use defaults)
2. Choose GitHub integration (recommended) or manual deploy
3. Wait 2-3 minutes
4. Done! Your app is live

## Option 2: One Command Deployment

### With GitHub (Auto-Deploy):

```bash
az staticwebapp create \
  --name recipecalculator \
  --resource-group RecipeCalculatorRG \
  --source https://github.com/YOUR_USERNAME/RecipeCalculator \
  --branch main \
  --location "East US 2" \
  --app-location "/src/RecipeCalculator.UI" \
  --output-location "wwwroot" \
  --login-with-github
```

Replace `YOUR_USERNAME` with your GitHub username.

### Without GitHub (Manual Deploy):

```bash
# 1. Create the app
az staticwebapp create \
  --name recipecalculator \
  --resource-group RecipeCalculatorRG \
  --location "East US 2"

# 2. Build locally
dotnet publish src/RecipeCalculator.UI/RecipeCalculator.UI.csproj -c Release -o publish

# 3. Get deployment token
TOKEN=$(az staticwebapp secrets list \
  --name recipecalculator \
  --resource-group RecipeCalculatorRG \
  --query "properties.apiKey" -o tsv)

# 4. Deploy (requires SWA CLI: npm install -g @azure/static-web-apps-cli)
swa deploy ./publish/wwwroot --deployment-token $TOKEN --env production
```

## Option 3: Via Azure Portal (GUI)

1. Go to [Azure Portal](https://portal.azure.com)
2. Click "+ Create a resource"
3. Search "Static Web Apps"
4. Click "Create"
5. Fill in:
   - **Name:** recipecalculator
   - **Plan:** Free
   - **Region:** Choose closest to you
   - **Source:** GitHub (or None for manual)
6. If GitHub:
   - Connect your repository
   - Set app location: `/src/RecipeCalculator.UI`
   - Set output location: `wwwroot`
7. Click "Review + Create"
8. Wait 2-3 minutes for deployment

## Your App is Live!

After deployment, your app will be available at:
```
https://recipecalculator.azurestaticapps.net
```

Or your custom URL if configured.

## What Happens Next?

### With GitHub Integration:
- ✅ GitHub Actions workflow created automatically
- ✅ Every push to `main` triggers auto-deployment
- ✅ Pull requests get staging environments
- ✅ Zero manual work needed

### Manual Deployment:
- Run `swa deploy` each time you want to update
- Or set up your own CI/CD pipeline

## Add Custom Domain (Optional)

```bash
# 1. Add domain to Static Web App
az staticwebapp hostname set \
  --name recipecalculator \
  --resource-group RecipeCalculatorRG \
  --hostname calculator.yourdomain.com

# 2. Add CNAME record in your DNS:
# Type: CNAME
# Name: calculator
# Value: recipecalculator.azurestaticapps.net
# TTL: 3600

# SSL is automatic and FREE!
```

## Monitoring

View your app in Azure Portal:
```bash
az staticwebapp show \
  --name recipecalculator \
  --resource-group RecipeCalculatorRG
```

Or visit:
```
https://portal.azure.com → Static Web Apps → recipecalculator
```

## Cost

**FREE FOREVER** for typical usage:
- 100 GB bandwidth/month
- Unlimited requests
- Custom domains + SSL
- Global CDN

Only pay if you exceed limits (~$9/month for Standard plan).

## Troubleshooting

### "Resource group not found"
```bash
az group create --name RecipeCalculatorRG --location eastus2
```

### "Not logged in"
```bash
az login
```

### "Build failed"
Check GitHub Actions tab in your repository for logs.

### "404 on routes"
The `staticwebapp.config.json` handles routing automatically.

### "Missing Azure secret in GitHub Actions"
If you want to skip deployment when the secret is missing (e.g., in forks):
1. Go to Settings → Secrets and variables → Actions → Variables
2. Add `SKIP_DEPLOY_ON_MISSING_SECRETS` = `true`
3. Builds will succeed but skip deployment

## Full Documentation

For advanced scenarios, see: [AZURE_DEPLOYMENT.md](./AZURE_DEPLOYMENT.md)

---

**That's it!** Deploy once, auto-updates forever. FREE. 🎉
