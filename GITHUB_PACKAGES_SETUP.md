# GitHub Packages Setup Guide

This document explains how GitHub Packages authentication is configured in this repository.

## Overview

This repository depends on NuGet packages from the AGTec namespace that are hosted on GitHub Packages:
- `AGTec.Common.Base` (v1.0.0)
- `AGTec.Common.Randomizer` (v1.0.0)
- `AGTec.Common.Test` (v1.0.0)

These packages are not available on nuget.org and require authentication to access from GitHub Packages.

## Repository Configuration

### NuGet.Config

The repository includes a `NuGet.Config` file at the root that configures two package sources:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="github" value="https://nuget.pkg.github.com/victorgreggio/index.json" />
  </packageSources>
</configuration>
```

This configuration:
1. Clears any inherited sources
2. Adds the official nuget.org source
3. Adds the GitHub Packages source for the victorgreggio organization

## GitHub Actions Setup

The GitHub Actions workflow (`.github/workflows/azure-static-web-apps.yml`) automatically authenticates with GitHub Packages using the built-in `GITHUB_TOKEN`:

```yaml
- name: Setup .NET
  uses: actions/setup-dotnet@v3
  with:
    dotnet-version: '9.0.x'

- name: Authenticate with GitHub Packages
  run: |
    dotnet nuget update source github --username victorgreggio --password ${{ secrets.GITHUB_TOKEN }} --store-password-in-clear-text

- name: Restore dependencies
  run: dotnet restore
```

The `GITHUB_TOKEN` is automatically provided by GitHub Actions and has the necessary permissions to read packages from the same repository organization.

## Local Development Setup

To build and run the project locally, you need to authenticate with GitHub Packages:

### Step 1: Create a Personal Access Token

1. Go to GitHub Settings → Developer settings → Personal access tokens → Tokens (classic)
2. Click "Generate new token (classic)"
3. Give it a descriptive name (e.g., "RecipeCalculator NuGet Access")
4. Select the `read:packages` scope
5. Click "Generate token"
6. Copy the token immediately (you won't be able to see it again)

### Step 2: Configure NuGet Authentication

Run the following command, replacing the placeholders with your actual values:

```bash
dotnet nuget update source github --username YOUR_GITHUB_USERNAME --password YOUR_GITHUB_TOKEN --store-password-in-clear-text
```

Example:
```bash
dotnet nuget update source github --username johndoe --password ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx --store-password-in-clear-text
```

### Step 3: Build the Project

Now you can restore packages and build the project:

```bash
dotnet restore
dotnet build
```

## Troubleshooting

### Error: Unable to find package AGTec.Common.Base

This error means you haven't authenticated with GitHub Packages. Follow the "Local Development Setup" steps above.

### Error: Response status code does not indicate success: 401 (Unauthorized)

This means your authentication credentials are invalid or expired:
1. Verify your Personal Access Token is still valid
2. Check that your token has the `read:packages` scope
3. Ensure you used the correct username and token in the authentication command
4. Try regenerating your token and authenticating again

### Error: Package source 'github' not found

This means the NuGet.Config file is not being recognized. Make sure:
1. The `NuGet.Config` file exists in the repository root
2. You are running commands from the repository root directory
3. Try running `dotnet nuget list source` to see registered sources

## Security Notes

- Never commit your Personal Access Token to the repository
- The `--store-password-in-clear-text` flag stores the password in plain text in your user NuGet.Config file (typically at `~/.nuget/NuGet/NuGet.Config` on Linux/Mac or `%APPDATA%\NuGet\NuGet.Config` on Windows)
- For better security, consider using a credential provider or the GitHub CLI for authentication
- Tokens should have minimal required permissions (only `read:packages` for this use case)

## Alternative Authentication Methods

### Using GitHub CLI

If you have the GitHub CLI installed and authenticated:

```bash
gh auth login
# Then the dotnet CLI can use the gh credentials automatically
```

### Using Environment Variables

You can also set credentials via environment variables:

```bash
export NUGET_AUTH_TOKEN=your_github_token
dotnet restore
```

## References

- [GitHub Packages Documentation](https://docs.github.com/en/packages)
- [NuGet Configuration Files](https://docs.microsoft.com/en-us/nuget/reference/nuget-config-file)
- [dotnet nuget commands](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-nuget-add-source)
