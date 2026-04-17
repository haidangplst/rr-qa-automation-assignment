@echo off
REM Setup script for QA Automation Framework
REM Installs Playwright browsers and dependencies

echo.
echo ========================================
echo  QA Automation Framework Setup
echo ========================================
echo.

echo Installing Playwright browsers...
call npx playwright install chromium
if errorlevel 1 (
    echo Warning: Could not install Playwright browsers via npx
    echo Try installing globally: npm install -g @playwright/test
)

echo.
echo Restoring .NET dependencies...
dotnet restore
if errorlevel 1 (
    echo Error: Failed to restore dependencies
    exit /b 1
)

echo.
echo Building project...
dotnet build
if errorlevel 1 (
    echo Error: Failed to build project
    exit /b 1
)

echo.
echo ========================================
echo  Setup Complete!
echo ========================================
echo.
echo You can now run tests with:
echo   dotnet test
echo.
echo For more information, see FRAMEWORK_GUIDE.md
echo.
pause
