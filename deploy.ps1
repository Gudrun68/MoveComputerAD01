# 🚀 Deployment-Skript für AD Computer Mover
# Automatisierte Veröffentlichung der Anwendung

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Test", "Production", "Local")]
    [string]$Environment = "Local",
    
    [Parameter(Mandatory=$false)]
    [string]$Version = "2.2.0.1",
    
    [Parameter(Mandatory=$false)]
    [string]$PublishPath = ".\publish"
)

Write-Host "🚀 AD Computer Mover - Deployment Script" -ForegroundColor Green
Write-Host "Environment: $Environment" -ForegroundColor Cyan
Write-Host "Version: $Version" -ForegroundColor Cyan
Write-Host "Publish Path: $PublishPath" -ForegroundColor Cyan
Write-Host ""

# 1. Clean previous builds
Write-Host "🧹 Bereinige vorherige Builds..." -ForegroundColor Yellow
if (Test-Path "bin") { Remove-Item "bin" -Recurse -Force }
if (Test-Path "obj") { Remove-Item "obj" -Recurse -Force }
if (Test-Path $PublishPath) { Remove-Item $PublishPath -Recurse -Force }

# 2. Restore packages
Write-Host "📦 Restore NuGet Packages..." -ForegroundColor Yellow
& nuget restore MoveComputerAD01.sln

# 3. Build solution
Write-Host "🔨 Erstelle Release Build..." -ForegroundColor Yellow
& msbuild MoveComputerAD01.sln /p:Configuration=Release /p:Platform="Any CPU" /verbosity:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build fehlgeschlagen!" -ForegroundColor Red
    exit 1
}

# 4. ClickOnce Publish
Write-Host "📤 Veröffentliche mit ClickOnce..." -ForegroundColor Yellow
& msbuild MoveComputerAD01.csproj /t:Publish /p:Configuration=Release /p:PublishUrl=$PublishPath /p:ApplicationVersion=$Version

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ ClickOnce Publish fehlgeschlagen!" -ForegroundColor Red
    exit 1
}

# 5. Copy documentation
Write-Host "📚 Kopiere Dokumentation..." -ForegroundColor Yellow
$docsSource = "docs"
$docsTarget = Join-Path $PublishPath "Application Files\*\docs"

if (Test-Path $docsSource) {
    $appFilesPath = Get-ChildItem -Path $PublishPath -Directory -Name "Application Files" -ErrorAction SilentlyContinue
    if ($appFilesPath) {
        $versionFolder = Get-ChildItem -Path (Join-Path $PublishPath "Application Files") -Directory | Sort-Object Name -Descending | Select-Object -First 1
        if ($versionFolder) {
            $targetDocsPath = Join-Path $versionFolder.FullName "docs"
            Copy-Item $docsSource -Destination $targetDocsPath -Recurse -Force
            Write-Host "✅ Dokumentation kopiert nach: $targetDocsPath" -ForegroundColor Green
        }
    }
}

# 6. Create version info
Write-Host "📝 Erstelle Versions-Info..." -ForegroundColor Yellow
$versionInfo = @{
    Version = $Version
    Environment = $Environment
    BuildDate = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
    BuildMachine = $env:COMPUTERNAME
    BuildUser = $env:USERNAME
    GitCommit = if (Get-Command git -ErrorAction SilentlyContinue) { 
        try { git rev-parse HEAD 2>$null } catch { "Unknown" }
    } else { "Git nicht verfügbar" }
    Files = @{
        MainExecutable = "MoveComputerAD01.exe"
        ConfigFile = "MoveComputerAD01.exe.config"
        DocumentationPath = "docs\"
    }
} | ConvertTo-Json -Depth 3

$versionInfo | Out-File (Join-Path $PublishPath "version.json") -Encoding UTF8

# 7. Create installation instructions
Write-Host "📋 Erstelle Installations-Anweisungen..." -ForegroundColor Yellow
$installInstructions = @"
# 🚀 Installation - AD Computer Mover v$Version

## Automatische Installation (empfohlen)
1. Doppelklick auf: setup.exe
2. Folgen Sie dem Installations-Assistenten
3. Die Anwendung wird automatisch gestartet

## Systemanforderungen
- Windows 10/11
- .NET Framework 4.8
- Domain-Zugang zu lps-berlin.de
- Administrator-Rechte (empfohlen)

## Nach der Installation
- Anwendung über Startmenü: "AD Computer Mover"
- Oder direkt: C:\Users\[User]\AppData\Local\Apps\2.0\...\MoveComputerAD01.exe

## Updates
- Automatische Updates beim Start (falls verfügbar)
- Aktuelle Version: $Version
- Build-Datum: $(Get-Date -Format 'dd.MM.yyyy HH:mm')

## Support
- IT-Support: it@lps-berlin.de
- Dokumentation: docs\Benutzerhandbuch.html

---
Erstellt am: $(Get-Date -Format 'dd.MM.yyyy HH:mm:ss')
Environment: $Environment
"@

$installInstructions | Out-File (Join-Path $PublishPath "INSTALLATION.md") -Encoding UTF8

# 8. Final summary
Write-Host ""
Write-Host "✅ Deployment erfolgreich abgeschlossen!" -ForegroundColor Green
Write-Host ""
Write-Host "📁 Veröffentlichung in: $PublishPath" -ForegroundColor Cyan
Write-Host "🔧 Setup-Datei: $(Join-Path $PublishPath 'setup.exe')" -ForegroundColor Cyan
Write-Host "📚 Dokumentation: $(Join-Path $PublishPath 'docs\')" -ForegroundColor Cyan
Write-Host "📋 Anweisungen: $(Join-Path $PublishPath 'INSTALLATION.md')" -ForegroundColor Cyan
Write-Host ""

# 9. Optional: Open publish folder
$openFolder = Read-Host "Möchten Sie den Publish-Ordner öffnen? (y/N)"
if ($openFolder -eq 'y' -or $openFolder -eq 'Y') {
    Start-Process -FilePath "explorer.exe" -ArgumentList $PublishPath
}

Write-Host "🎉 Deployment Script beendet!" -ForegroundColor Green
