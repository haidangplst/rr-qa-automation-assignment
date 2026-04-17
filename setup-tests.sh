#!/bin/bash

# Setup script for QA Automation Framework
# Installs Playwright browsers and dependencies

echo "Installing Playwright browsers..."
playwright install chromium

echo "Installing .NET dependencies..."
dotnet restore

echo "Building project..."
dotnet build

echo "Setup complete! You can now run tests with: dotnet test"
