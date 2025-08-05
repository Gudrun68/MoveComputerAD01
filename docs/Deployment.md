# 🚀 Deployment-Anleitung - AD Computer Mover

## 📋 Übersicht

Diese Anleitung beschreibt verschiedene Methoden zur Veröffentlichung der AD Computer Mover Anwendung.

## 🎯 Verfügbare Deployment-Methoden

### 1. ClickOnce-Veröffentlichung (empfohlen)

ClickOnce bietet automatische Updates und einfache Installation.

#### In Visual Studio:
1. **Rechtsklick auf Projekt** → "Veröffentlichen..."
2. **Veröffentlichungsort wählen**:
   - **Netzwerkfreigabe**: `\\dc-01\NETLOGON\MoveComputerAD01`
    - **Lokaler Pfad**: `C:\Deploy\MoveComputerAD01\`

3. **Konfiguration**:
   - ✅ **Online/Offline**: Online und Offline verfügbar
   - ✅ **Updates**: Automatisch bei Start prüfen
   - ✅ **Voraussetzungen**: .NET Framework 4.8
   - ✅ **Sicherheit**: Vollvertrauen erforderlich
   - ✅ **Icon**: Anwendungsicon (app.ico) enthalten

#### Über PowerShell:
```powershell
# ClickOnce veröffentlichen
msbuild MoveComputerAD01.sln /t:publish /p:Configuration=Release /p:PublishUrl="\\server\share\MoveComputerAD01\"
```

### 2. Standalone-EXE (ohne Installation)

#### Einfache Binärdateien:
```powershell
# Release Build erstellen
msbuild MoveComputerAD01.sln /p:Configuration=Release

# Dateien sammeln
$sourceDir = "bin\Release"
$deployDir = "Deploy\Standalone"
New-Item -ItemType Directory -Path $deployDir -Force

# Hauptanwendung
Copy-Item "$sourceDir\MoveComputerAD01.exe" -Destination $deployDir
Copy-Item "$sourceDir\MoveComputerAD01.exe.config" -Destination $deployDir

# Dependencies
Copy-Item "$sourceDir\System.DirectoryServices.dll" -Destination $deployDir -ErrorAction SilentlyContinue

# Dokumentation
Copy-Item "docs" -Destination "$deployDir\docs" -Recurse -Force
```

### 3. MSI-Installer erstellen

#### WiX Toolset verwenden:
```xml
<!-- setup.wxs -->
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <Product Id="*" Name="AD Computer Mover" Language="1033" Version="2.2.0.1" 
           Manufacturer="LPS-Berlin IT" UpgradeCode="YOUR-GUID-HERE">
    
    <Package InstallerVersion="200" Compressed="yes" InstallScope="perMachine" />
    
    <MajorUpgrade DowngradeErrorMessage="Eine neuere Version ist bereits installiert." />
    <MediaTemplate EmbedCab="yes" />
    
    <Feature Id="ProductFeature" Title="AD Computer Mover" Level="1">
      <ComponentGroupRef Id="ProductComponents" />
    </Feature>
  </Product>
  
  <Fragment>
    <Directory Id="TARGETDIR" Name="SourceDir">
      <Directory Id="ProgramFilesFolder">
        <Directory Id="INSTALLFOLDER" Name="AD Computer Mover" />
      </Directory>
    </Directory>
  </Fragment>
  
  <Fragment>
    <ComponentGroup Id="ProductComponents" Directory="INSTALLFOLDER">
      <Component Id="MoveComputerAD01.exe">
        <File Source="bin\Release\MoveComputerAD01.exe" />
      </Component>
      <Component Id="MoveComputerAD01.exe.config">
        <File Source="bin\Release\MoveComputerAD01.exe.config" />
      </Component>
    </ComponentGroup>
  </Fragment>
</Wix>
```

### 4. MSIX-Package (moderne Windows-Apps)

```powershell
# MSIX Packaging Tool verwenden
# Oder mit Visual Studio Extension "Windows Application Packaging Project"
```

## 🔧 Deployment-Konfiguration

### App.config für verschiedene Umgebungen:

#### Produktionsumgebung:
```xml
<configuration>
  <appSettings>
    <add key="Environment" value="Production" />
    <add key="LDAPPath" value="LDAP://DC=lps-berlin,DC=de" />
    <add key="LogLevel" value="Info" />
    <add key="AutoUpdate" value="true" />
  </appSettings>
</configuration>
```

#### Testumgebung:
```xml
<configuration>
  <appSettings>
    <add key="Environment" value="Test" />
    <add key="LDAPPath" value="LDAP://DC=test,DC=lps-berlin,DC=de" />
    <add key="LogLevel" value="Debug" />
    <add key="AutoUpdate" value="false" />
  </appSettings>
</configuration>
```

## 📦 Automatisiertes Deployment

### PowerShell-Skript für vollautomatisches Deployment:

```powershell
# deploy.ps1
param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("Test", "Production")]
    [string]$Environment,
    
    [string]$Version = "2.2.0.1",
    [string]$DeployPath = "\\fileserver\Applications\MoveComputerAD01"
)

Write-Host "🚀 Starte Deployment für $Environment..." -ForegroundColor Green

# 1. Clean & Build
Write-Host "📦 Erstelle Release Build..." -ForegroundColor Yellow
& dotnet clean --configuration Release
& dotnet build --configuration Release --no-restore

# 2. Tests ausführen (falls vorhanden)
Write-Host "🧪 Führe Tests aus..." -ForegroundColor Yellow
# & dotnet test --no-build --configuration Release

# 3. Publish
Write-Host "📤 Veröffentliche Anwendung..." -ForegroundColor Yellow
$publishPath = "$DeployPath\$Environment\v$Version"
& dotnet publish --configuration Release --output $publishPath

# 4. Dokumentation kopieren
Write-Host "📚 Kopiere Dokumentation..." -ForegroundColor Yellow
Copy-Item "docs" -Destination "$publishPath\docs" -Recurse -Force

# 5. Version-Info erstellen
$versionInfo = @{
    Version = $Version
    Environment = $Environment
    BuildDate = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
    BuildMachine = $env:COMPUTERNAME
    GitCommit = (git rev-parse HEAD)
} | ConvertTo-Json

$versionInfo | Out-File "$publishPath\version.json" -Encoding UTF8

Write-Host "✅ Deployment abgeschlossen!" -ForegroundColor Green
Write-Host "📍 Pfad: $publishPath" -ForegroundColor Cyan
```

## 🔐 Sicherheitsüberlegungen

### Code Signing (empfohlen):
```powershell
# Signierung der EXE-Datei
$cert = Get-ChildItem -Path Cert:\CurrentUser\My -CodeSigningCert
Set-AuthenticodeSignature -FilePath "MoveComputerAD01.exe" -Certificate $cert
```

### Berechtigungen:
- **ClickOnce**: Vollvertrauen erforderlich für AD-Zugriff
- **MSI**: Administrator-Rechte für Installation
- **Standalone**: Keine besonderen Berechtigungen

## 📊 Deployment-Monitoring

### Log-Sammlung:
```powershell
# Deployment-Logs sammeln
$logPath = "$env:TEMP\MoveComputerAD01_Deploy.log"
Start-Transcript -Path $logPath

# ... Deployment-Schritte ...

Stop-Transcript
```

### Update-Mechanismus:
- **ClickOnce**: Automatische Updates
- **MSI**: Manueller Update über neue MSI
- **Standalone**: Manueller Austausch der EXE

## 🎯 Empfohlener Workflow

### Für interne Verteilung (LPS-Berlin):

1. **Development** → **Test-Environment**:
   ```powershell
   .\deploy.ps1 -Environment Test
   ```

2. **Test** → **Production**:
   ```powershell
   .\deploy.ps1 -Environment Production -Version 2.2.0.1
   ```

3. **Benutzer-Installation**:
   - Öffnen: `\\fileserver\Applications\MoveComputerAD01\Production\setup.exe`
   - Oder: Direkter Link auf Intranet-Seite

## 📞 Support bei Deployment-Problemen

### Häufige Probleme:
1. **ClickOnce-Vertrauensfehler**: Zertifikat oder Vollvertrauen-Berechtigung
2. **.NET Framework fehlt**: Bootstrapper-Package installieren
3. **AD-Berechtigung**: Anwendung als Administrator ausführen
4. **Netzwerk-Installation**: UNC-Pfad und Berechtigungen prüfen

### Kontakt:
- **IT-Support**: it@lps-berlin.de
- **Entwickler**: Via GitHub Issues

---

*Diese Anleitung wird kontinuierlich aktualisiert. Letzte Änderung: August 2025*
