# AD Computer Mover

Ein WPF-basiertes Tool zum Verwalten und Verschieben von Computerobjekten in Active Directory Organisationseinheiten (OUs).

## 🚀 Features

- **Intuitive Benutzeroberfläche**: Moderne WPF-Anwendung mit TreeView-Navigation
- **Sichere Authentifizierung**: Anmeldedialog für AD-Credentials mit Validierung
- **Drag & Drop Funktionalität**: Einfaches Verschieben von Computern zwischen OUs
- **Mehrfache Fallback-Strategien**: DirectoryServices + PowerShell Integration
- **Umfassende Protokollierung**: Detaillierte Logs aller Operationen
- **Professionelle Architektur**: Service-orientierte Struktur mit Separation of Concerns

## 🏗️ Architektur

```
MoveComputerAD01/
├── Utilities/        # Utility-Klassen und Helper-Funktionen
├── Dialogs/          # UI-Dialoge (Credentials)
├── EventHandlers/    # Event-Handler Logic
├── Models/           # Datenmodelle
├── Services/         # Business Logic (AD-Services)
├── ViewModels/       # MVVM ViewModels
├── Views/            # UI Views
├── Properties/       # Assembly-Eigenschaften
└── docs/             # Dokumentation
```

## 🛠️ Technische Anforderungen

- **Framework**: .NET Framework 4.8
- **UI**: WPF (Windows Presentation Foundation)
- **AD Integration**: System.DirectoryServices
- **PowerShell**: Fallback für AD-Operationen
- **IDE**: Visual Studio 2019/2022
- **OS**: Windows 10/11 mit Domain-Zugang

## 📋 Voraussetzungen

### System
- Windows 10 oder höher
- .NET Framework 4.8
- Domain-Mitgliedschaft oder AD-Zugang
- Administrator-Rechte (empfohlen)

### Active Directory
- Gültiger AD-Benutzer mit Computer-Verwaltungsrechten
- Zugang zur Ziel-Domain: `lps-berlin.de`
- LDAP-Zugriff auf Domain Controller

## ⚡ Schnellstart

1. **Repository klonen**
   ```bash
   git clone <repository-url>
   cd MoveComputerAD01
   ```

2. **Projekt öffnen**
   - Visual Studio öffnen
   - `MoveComputerAD01.sln` laden

3. **Kompilieren**
   ```bash
   msbuild MoveComputerAD01.sln /p:Configuration=Release
   ```

4. **Ausführen**
   ```bash
   .\bin\Release\MoveComputerAD01.exe
   ```

## 🔧 Konfiguration

### LDAP-Pfad anpassen
In `Services/ActiveDirectoryService.cs`:
```csharp
private const string LDAP_PATH = "LDAP://DC=lps-berlin,DC=de";
```

### Logging konfigurieren
Logs werden automatisch in der Anwendung angezeigt. Für erweiterte Konfiguration siehe `MainWindow.xaml.cs`.

## 🧪 Testing

Die Anwendung kann über die GUI getestet werden:

1. **Verbindungstest**: Beim Start wird die AD-Verbindung getestet
2. **Strukturtest**: AD-Objekte werden in TreeViews geladen
3. **Operationstest**: Computer zwischen OUs verschieben

## 📁 Projektstruktur im Detail

### Core Services
- **ActiveDirectoryService**: Haupt-AD-Operations-Service
- **MainWindowEventHandlers**: Event-Handling-Logik

### Models
- **ADObject**: Repräsentation von AD-Objekten
- **ComputerMoveResult**: Ergebnis von Move-Operationen

### UI Components
- **MainWindow**: Hauptfenster mit TreeViews
- **CredentialsDialog**: Sichere Anmeldung
- **InputDialog**: Allgemeine Eingabedialoge

## 🔒 Sicherheit

- Sichere Credential-Eingabe mit SecureString
- Validierung aller Benutzereingaben
- Fehlerbehandlung für fehlgeschlagene AD-Operationen
- Administrator-Rechte-Prüfung

## 🐛 Debugging

### Häufige Probleme

1. **LDAP-Verbindungsfehler**
   - Domain Controller erreichbar?
   - Credentials korrekt?
   - Firewall-Einstellungen prüfen

2. **Berechtigung verweigert**
   - Benutzer hat Computer-Verwaltungsrechte?
   - Als Administrator ausführen

3. **PowerShell-Fallback**
   - ActiveDirectory PowerShell-Modul installiert?
   - Execution Policy korrekt gesetzt?

### Log-Analyse
Die Anwendung protokolliert alle Operationen im unteren Bereich des Hauptfensters.

## 🤝 Mitwirken

1. Fork erstellen
2. Feature-Branch erstellen (`git checkout -b feature/AmazingFeature`)
3. Änderungen committen (`git commit -m 'Add some AmazingFeature'`)
4. Branch pushen (`git push origin feature/AmazingFeature`)
5. Pull Request erstellen

## 📄 Lizenz

Dieses Projekt ist für interne Nutzung bei LPS-Berlin entwickelt.

## 👥 Autoren

- **Entwickler**: Erstellt für LPS-Berlin AD-Verwaltung
- **Kontakt**: IT-Abteilung LPS-Berlin

## 🔄 Versionierung

- **v1.0**: Initiale Version mit grundlegenden AD-Operationen
- **v2.0**: Refaktorierung mit professioneller Architektur
- **v2.1**: Credential-Dialog und verbesserte Fehlerbehandlung

## 📞 Support

Bei Problemen oder Fragen:
1. Logs aus der Anwendung sammeln
2. IT-Support kontaktieren
3. Issue im Repository erstellen

---

*Entwickelt für die effiziente Verwaltung von Computer-Objekten in der LPS-Berlin Active Directory Umgebung.*
