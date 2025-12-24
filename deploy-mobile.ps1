# EmptySlot Mobile Deployment Script
# Run this to build and deploy the mobile app

Write-Host "Building and deploying EmptySlot.Mobile..." -ForegroundColor Cyan

# Navigate to project directory
Set-Location "$PSScriptRoot\dotnet"

# Build and deploy to Android emulator
dotnet build EmptySlot.Mobile -f net9.0-android -t:Run

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nApp deployed successfully! " -ForegroundColor Green
    Write-Host "Check your Android emulator to see the app." -ForegroundColor Yellow
} else {
    Write-Host "`nDeployment failed! " -ForegroundColor Red
}
