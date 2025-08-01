# Installation und Setup

## 💻 Systemanforderungen

### Minimum-Anforderungen
- Windows 10 (Version 1909 oder höher)
- .NET Framework 4.8
- 512 MB RAM
- 50 MB freier Festplattenspeicher
- Netzwerk-Zugang zum Domain Controller

### Empfohlene Anforderungen
- Windows 11 oder Windows Server 2019+
- 2 GB RAM
- SSD-Festplatte
- Gigabit-Netzwerk-Verbindung

## 🚀 Installation

### Option 1: Vorkompilierte Version

1. **Download**
   - Laden Sie die neueste Version von `[Release-URL]` herunter
   - Entpacken Sie die ZIP-Datei in gewünschten Ordner

2. **Erste Ausführung**
   ```cmd
   cd C:\Programme\ADComputerMover
   MoveComputerAD01.exe
   ```

### Option 2: Aus Quellcode kompilieren

1. **Voraussetzungen installieren**
   - Visual Studio 2019/2022 Community (oder höher)
   - .NET Framework 4.8 SDK

2. **Repository klonen**
   ```bash
   git clone [Repository-URL]
   cd MoveComputerAD01
   ```

3. **Kompilieren**
   ```cmd
   # Via Visual Studio
   # Öffnen Sie MoveComputerAD01.sln und drücken Sie F5

   # Via MSBuild (Kommandozeile)
   msbuild MoveComputerAD01.sln /p:Configuration=Release
   ```

## ⚙️ Konfiguration

### Domain-Einstellungen anpassen

Falls Sie eine andere Domain verwenden, passen Sie die Konfiguration an:

**Datei**: `Services/ActiveDirectoryService.cs`
```csharp
private const string LDAP_PATH = "LDAP://DC=ihre-domain,DC=de";
```

### PowerShell-Module (optional)

Für erweiterte Funktionalität installieren Sie das ActiveDirectory-Modul:

```powershell
# Als Administrator ausführen
Install-WindowsFeature -Name "RSAT-AD-PowerShell" -IncludeAllSubFeature
```

### Firewall-Konfiguration

Stellen Sie sicher, dass folgende Ports geöffnet sind:
- **389** (LDAP)
- **636** (LDAPS)
- **3268** (Global Catalog)

## 🔐 Berechtigung

### Active Directory Berechtigung

Der verwendete Benutzer benötigt:
- **Lesen**: Alle Computer-Objekte
- **Schreiben**: Computer-Objekte verschieben
- **Erweiterte Berechtigung**: Create/Delete Computer Objects (in Ziel-OUs)

### Windows-Berechtigung

- Mitgliedschaft in lokaler Gruppe "Benutzer"
- Optional: Administrator-Rechte für erweiterte Funktionen

## 🧪 Installationstest

### Schneller Funktionstest

1. **Anwendung starten**
2. **Anmeldung mit Test-Account**
3. **AD-Struktur laden lassen**
4. **Test-Computer auswählen** (ohne zu verschieben)

### Umfassender Test

```powershell
# PowerShell-Test der AD-Verbindung
Import-Module ActiveDirectory
Get-ADComputer -Filter * -SearchBase "OU=Computers,DC=lps-berlin,DC=de"
```

## 📁 Verzeichnisstruktur nach Installation

```
C:\Programme\ADComputerMover\
├── MoveComputerAD01.exe           # Hauptanwendung
├── MoveComputerAD01.exe.config    # Konfiguration
├── MoveComputerAD01.pdb           # Debug-Informationen
├── docs\                          # Dokumentation
│   ├── Benutzerhandbuch.md
│   ├── Installation.md
│   └── Troubleshooting.md
└── logs\                          # Log-Dateien (automatisch erstellt)
    └── application.log
```

## 🔄 Updates

### Automatische Updates (geplant)
- Updates werden über internes Repository verteilt
- Benachrichtigung bei neuer Version

### Manuelle Updates
1. Aktuelle Version herunterladen
2. Anwendung schließen
3. Neue Dateien überschreiben
4. Anwendung neu starten

## 🗑️ Deinstallation

1. **Anwendung schließen**
2. **Installationsordner löschen**
3. **Optional**: Registry-Einträge löschen
   ```cmd
   reg delete "HKCU\Software\LPS-Berlin\ADComputerMover" /f
   ```

## 🔧 Troubleshooting Installation

### Problem: .NET Framework fehlt
```cmd
# Download und Installation von Microsoft
# https://dotnet.microsoft.com/download/dotnet-framework/net48
```

### Problem: Keine Berechtigung zur Installation
- Als Administrator anmelden
- Oder IT-Administrator um Installation bitten

### Problem: Antivirus blockiert Anwendung
- Ausnahme für `MoveComputerAD01.exe` hinzufügen
- Oder digitale Signatur prüfen lassen

---

## 📞 Support bei Installation

**IT-Helpdesk LPS-Berlin**
- **E-Mail**: it-helpdesk@lps-berlin.de
- **Telefon**: [Interne Nummer]
- **Ticket**: [Ticket-System URL]

**Häufige Support-Fragen:**
1. Wo finde ich die Installationsdatei?
2. Welche Berechtigung brauche ich?
3. Funktioniert es auf meinem System?
4. Wie aktualisiere ich die Anwendung?

---

*Installation getestet auf Windows 10/11 und Windows Server 2019/2022*
