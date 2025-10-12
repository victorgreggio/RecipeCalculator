#!/bin/bash
# Azure Static Web Apps Deployment Script for RecipeCalculator

set -e

echo "======================================"
echo "RecipeCalculator - Azure Deployment"
echo "======================================"
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    echo -e "${RED}Error: Azure CLI is not installed${NC}"
    echo "Install from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
fi

# Check if logged in
if ! az account show &> /dev/null; then
    echo -e "${YELLOW}Not logged in to Azure. Logging in...${NC}"
    az login
fi

# Get parameters
read -p "Enter Resource Group name (default: RecipeCalculatorRG): " RESOURCE_GROUP
RESOURCE_GROUP=${RESOURCE_GROUP:-RecipeCalculatorRG}

read -p "Enter Static Web App name (default: recipecalculator): " APP_NAME
APP_NAME=${APP_NAME:-recipecalculator}

read -p "Enter Azure region (default: eastus2): " LOCATION
LOCATION=${LOCATION:-eastus2}

read -p "Do you have a GitHub repository? (y/n): " HAS_GITHUB

read -p "Skip deployment if Azure secret is missing? (y/n, default: n): " SKIP_DEPLOY_ON_MISSING_SECRETS
SKIP_DEPLOY_ON_MISSING_SECRETS=${SKIP_DEPLOY_ON_MISSING_SECRETS:-n}

echo ""
echo -e "${GREEN}Configuration:${NC}"
echo "  Resource Group: $RESOURCE_GROUP"
echo "  App Name: $APP_NAME"
echo "  Location: $LOCATION"
echo "  Skip on missing secrets: $SKIP_DEPLOY_ON_MISSING_SECRETS"
echo ""

# Create resource group
echo -e "${YELLOW}Creating resource group...${NC}"
az group create --name "$RESOURCE_GROUP" --location "$LOCATION" || true

if [ "$HAS_GITHUB" = "y" ] || [ "$HAS_GITHUB" = "Y" ]; then
    read -p "Enter GitHub repository URL (e.g., https://github.com/user/repo): " GITHUB_REPO
    read -p "Enter branch name (default: main): " BRANCH
    BRANCH=${BRANCH:-main}
    
    echo -e "${YELLOW}Creating Static Web App with GitHub integration...${NC}"
    az staticwebapp create \
        --name "$APP_NAME" \
        --resource-group "$RESOURCE_GROUP" \
        --source "$GITHUB_REPO" \
        --location "$LOCATION" \
        --branch "$BRANCH" \
        --app-location "/src/RecipeCalculator.UI" \
        --output-location "wwwroot" \
        --login-with-github
    
    echo ""
    echo -e "${GREEN}✓ Static Web App created with GitHub Actions!${NC}"
    echo -e "${GREEN}✓ GitHub Actions workflow added to your repository${NC}"
    echo -e "${GREEN}✓ Every push to '$BRANCH' will auto-deploy${NC}"
else
    echo -e "${YELLOW}Creating Static Web App without GitHub integration...${NC}"
    az staticwebapp create \
        --name "$APP_NAME" \
        --resource-group "$RESOURCE_GROUP" \
        --location "$LOCATION"
    
    # Get deployment token
    echo ""
    echo -e "${YELLOW}Getting deployment token...${NC}"
    DEPLOYMENT_TOKEN=$(az staticwebapp secrets list \
        --name "$APP_NAME" \
        --resource-group "$RESOURCE_GROUP" \
        --query "properties.apiKey" -o tsv 2>/dev/null)
    
    if [ -z "$DEPLOYMENT_TOKEN" ] || [ "$DEPLOYMENT_TOKEN" = "null" ]; then
        if [ "$SKIP_DEPLOY_ON_MISSING_SECRETS" = "y" ] || [ "$SKIP_DEPLOY_ON_MISSING_SECRETS" = "Y" ]; then
            echo -e "${YELLOW}Warning: Azure deployment token is missing. Skipping deployment as requested.${NC}"
            echo ""
            echo -e "${GREEN}======================================${NC}"
            echo -e "${GREEN}Setup Complete (Deployment Skipped)${NC}"
            echo -e "${GREEN}======================================${NC}"
            exit 0
        else
            echo -e "${RED}Error: Failed to get deployment token from Azure.${NC}"
            echo -e "${RED}The Static Web App may not exist or you may not have permissions.${NC}"
            exit 1
        fi
    fi
    
    echo ""
    echo -e "${YELLOW}Building application...${NC}"
    echo -e "${YELLOW}Note: Ensure you have authenticated with GitHub Packages.${NC}"
    echo -e "${YELLOW}Run: dotnet nuget update source github --username YOUR_USERNAME --password YOUR_TOKEN --store-password-in-clear-text${NC}"
    dotnet publish src/RecipeCalculator.UI/RecipeCalculator.UI.csproj -c Release -o publish
    
    echo ""
    echo -e "${YELLOW}Deploying to Azure...${NC}"
    
    # Check if SWA CLI is installed
    if ! command -v swa &> /dev/null; then
        echo -e "${YELLOW}Installing Azure Static Web Apps CLI...${NC}"
        npm install -g @azure/static-web-apps-cli
    fi
    
    swa deploy ./publish/wwwroot \
        --deployment-token "$DEPLOYMENT_TOKEN" \
        --env production
    
    echo ""
    echo -e "${GREEN}✓ Application deployed!${NC}"
fi

# Get the URL
echo ""
echo -e "${YELLOW}Getting app URL...${NC}"
APP_URL=$(az staticwebapp show \
    --name "$APP_NAME" \
    --resource-group "$RESOURCE_GROUP" \
    --query "defaultHostname" -o tsv)

echo ""
echo -e "${GREEN}======================================"
echo "Deployment Complete!"
echo "======================================${NC}"
echo ""
echo -e "Your app is live at: ${GREEN}https://$APP_URL${NC}"
echo ""
echo "Next steps:"
echo "  1. Visit your app: https://$APP_URL"
echo "  2. Configure custom domain (optional):"
echo "     az staticwebapp hostname set --name $APP_NAME --hostname yourdomain.com"
echo "  3. Monitor in Azure Portal:"
echo "     https://portal.azure.com/#resource/subscriptions/.../resourceGroups/$RESOURCE_GROUP/providers/Microsoft.Web/staticSites/$APP_NAME"
echo ""
