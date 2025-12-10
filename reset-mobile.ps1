# Complete Mobile App Reset Script for Windows PowerShell
# Run this from the EmtpySlot root directory

Write-Host "🔄 Starting complete mobile app reset..." -ForegroundColor Cyan

# Stop any running processes
Write-Host "`n📛 Please close any running Metro bundlers (Ctrl+C in terminals)" -ForegroundColor Yellow
Write-Host "Press Enter when ready..."
$null = Read-Host

# 1. Delete all node_modules
Write-Host "`n🗑️  Removing all node_modules..." -ForegroundColor Cyan
Remove-Item -Recurse -Force node_modules -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force apps\mobile\node_modules -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force apps\web\node_modules -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force packages\shared\node_modules -ErrorAction SilentlyContinue

# 2. Delete all lock files
Write-Host "🗑️  Removing lock files..." -ForegroundColor Cyan
Remove-Item -Force package-lock.json -ErrorAction SilentlyContinue
Remove-Item -Force apps\mobile\package-lock.json -ErrorAction SilentlyContinue
Remove-Item -Force apps\web\package-lock.json -ErrorAction SilentlyContinue
Remove-Item -Force packages\shared\package-lock.json -ErrorAction SilentlyContinue

# 3. Clear all caches
Write-Host "🗑️  Clearing caches..." -ForegroundColor Cyan
Remove-Item -Recurse -Force apps\mobile\.expo -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force apps\mobile\.cache -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force $env:TEMP\metro-* -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force $env:TEMP\react-* -ErrorAction SilentlyContinue

# 4. Reinstall from root (this handles the workspace correctly)
Write-Host "`n📦 Installing dependencies from root..." -ForegroundColor Cyan
npm install

# 5. Verify mobile app dependencies
Write-Host "`n✅ Verifying mobile app installation..." -ForegroundColor Cyan
Set-Location apps\mobile
npm install
Set-Location ..\..

# 6. Clear Metro bundler cache
Write-Host "`n🧹 Clearing Metro bundler cache..." -ForegroundColor Cyan
Set-Location apps\mobile
npx react-native start --reset-cache --no-interactive &
Start-Sleep -Seconds 5
Stop-Process -Name "node" -Force -ErrorAction SilentlyContinue
Set-Location ..\..

Write-Host "`n✅ Reset complete!" -ForegroundColor Green
Write-Host "`n📱 To start the mobile app:" -ForegroundColor Yellow
Write-Host "   cd apps\mobile" -ForegroundColor White
Write-Host "   npx expo start --clear" -ForegroundColor White
Write-Host "`nThen press 'r' to reload in your app" -ForegroundColor Yellow
