# Troubleshooting & FAQ

## 🚨 Häufige Probleme und Lösungen

### 1. Anmelde-Probleme

#### Problem: "Anmeldung fehlgeschlagen - Ungültige Anmeldeinformationen"

**Mögliche Ursachen:**
- Falscher Benutzername oder Passwort
- Domain nicht erreichbar
- Account gesperrt oder abgelaufen

**Lösungsschritte:**
1. **Credentials prüfen**
   ```cmd
   # Testen Sie die Anmeldung über Kommandozeile
   runas /user:lps-berlin\[username] cmd
   ```

2. **Domain-Verbindung testen**
   ```cmd
   ping lps-berlin.de
   nslookup lps-berlin.de
   ```

3. **Account-Status prüfen**
   - Mit IT-Administrator sprechen
   - Account auf Sperrung prüfen

#### Problem: "Domain Controller nicht erreichbar"

**Diagnose:**
```powershell
# Domain Controller finden
nslookup -type=srv _ldap._tcp.lps-berlin.de

# LDAP-Port testen
Test-NetConnection -ComputerName [DC-Name] -Port 389
```

**Lösung:**
- VPN-Verbindung prüfen
- Firewall-Einstellungen kontrollieren
- DNS-Einstellungen validieren

### 2. Computer-Verwaltung

#### Problem: "Computer nicht gefunden"

**Debugging-Schritte:**
1. **Computer-Existenz prüfen**
   ```powershell
   Get-ADComputer -Identity "COMPUTER-NAME"
   ```

2. **Such-Scope erweitern**
   ```csharp
   // Im Code: Erweiterte Suche aktivieren
   searcher.SearchScope = SearchScope.Subtree;
   ```

3. **Berechtigung validieren**
   ```powershell
   # Aktuelle Berechtigung prüfen
   whoami /groups
   ```

#### Problem: "Berechtigung verweigert beim Verschieben"

**Benötigte Berechtigung:**
- **Move/Rename**: Computer-Objekte
- **Write**: Ziel-OU
- **Delete**: Quell-OU (bei Cross-Domain)

**Lösung:**
```powershell
# Berechtigung für OU prüfen
dsacls "OU=Computers,DC=lps-berlin,DC=de"
```

### 3. PowerShell-Fallback Probleme

#### Problem: "PowerShell-Modul nicht gefunden"

**Installation:**
```powershell
# Als Administrator ausführen
Install-WindowsFeature -Name "RSAT-AD-PowerShell"

# Alternative: RSAT Tools
# Download von Microsoft.com
```

#### Problem: "Execution Policy blockiert Skript"

**Lösung:**
```powershell
# Execution Policy temporär ändern
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Permanent (als Admin)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope LocalMachine
```

### 4. Performance-Probleme

#### Problem: "Anwendung reagiert nicht beim Laden"

**Diagnose:**
- Große AD-Struktur (>1000 Computer)
- Langsame Netzwerk-Verbindung
- Überlasteter Domain Controller

**Optimierung:**
```csharp
// Paging für große Resultsets
searcher.PageSize = 1000;
searcher.SizeLimit = 5000;

// Nur benötigte Properties laden
searcher.PropertiesToLoad.AddRange(new[] { "name", "distinguishedName" });
```

#### Problem: "UI friert ein"

**Lösung:**
```csharp
// Async/Await verwenden
private async void LoadADStructure()
{
    try
    {
        var structures = await _adService.LoadADStructureAsync();
        // UI Update im UI Thread
        Dispatcher.Invoke(() => PopulateTreeView(structures));
    }
    catch (Exception ex)
    {
        LogError($"Fehler beim Laden: {ex.Message}");
    }
}
```

## 🔧 Erweiterte Diagnose

### LDAP-Debugging

#### Detaillierte LDAP-Fehler
```csharp
try
{
    // LDAP-Operation
}
catch (DirectoryServicesCOMException ex)
{
    switch (ex.ExtendedError)
    {
        case 0x52e: // ERROR_LOGON_FAILURE
            Console.WriteLine("Anmeldung fehlgeschlagen");
            break;
        case 0x525: // ERROR_NO_SUCH_USER
            Console.WriteLine("Benutzer nicht gefunden");
            break;
        case 0x52f: // ERROR_ACCOUNT_RESTRICTION
            Console.WriteLine("Account-Einschränkung");
            break;
        default:
            Console.WriteLine($"LDAP-Fehler: {ex.ExtendedError:X}");
            break;
    }
}
```

#### LDAP-Pfad Validierung
```csharp
public static bool IsValidLdapPath(string ldapPath)
{
    try
    {
        using (var entry = new DirectoryEntry(ldapPath))
        {
            var test = entry.Name; // Trigger LDAP access
            return true;
        }
    }
    catch
    {
        return false;
    }
}
```

### Netzwerk-Diagnose

#### Port-Konnektivität testen
```powershell
# LDAP (389)
Test-NetConnection -ComputerName dc.lps-berlin.de -Port 389

# LDAPS (636)
Test-NetConnection -ComputerName dc.lps-berlin.de -Port 636

# Global Catalog (3268)
Test-NetConnection -ComputerName dc.lps-berlin.de -Port 3268
```

#### DNS-Auflösung prüfen
```cmd
# Domain Controller SRV Records
nslookup -type=srv _ldap._tcp.lps-berlin.de
nslookup -type=srv _kerberos._tcp.lps-berlin.de

# A-Records
nslookup dc.lps-berlin.de
```

## 📊 Logging und Monitoring

### Debug-Logging aktivieren

```csharp
// In App.config
<system.diagnostics>
  <trace autoflush="true">
    <listeners>
      <add name="textwriterListener" 
           type="System.Diagnostics.TextWriterTraceListener" 
           initializeData="debug.log" />
    </listeners>
  </trace>
</system.diagnostics>

// Im Code
Trace.WriteLine($"LDAP-Verbindung: {ldapPath}");
Trace.WriteLine($"Benutzer: {username}");
```

### Event Log Einträge

```csharp
using (var eventLog = new EventLog("Application"))
{
    eventLog.Source = "ADComputerMover";
    eventLog.WriteEntry(
        "Computer erfolgreich verschoben", 
        EventLogEntryType.Information
    );
}
```

## 🛠️ Konfiguration und Anpassung

### Domain-Einstellungen

**Datei**: `Services/ActiveDirectoryService.cs`
```csharp
// Standard-Konfiguration
private const string LDAP_PATH = "LDAP://DC=lps-berlin,DC=de";

// Für andere Domains anpassen
private const string LDAP_PATH = "LDAP://DC=firma,DC=local";
```

### Timeout-Einstellungen

```csharp
// DirectorySearcher Timeouts
searcher.ClientTimeout = TimeSpan.FromMinutes(2);
searcher.ServerTimeLimit = TimeSpan.FromMinutes(1);

// PowerShell Timeouts
var ps = PowerShell.Create();
ps.Runspace.SessionStateProxy.SetVariable("ErrorActionPreference", "Stop");
```

### Custom LDAP-Filter

```csharp
// Nur aktive Computer
searcher.Filter = "(&(objectClass=computer)(!(userAccountControl:1.2.840.113556.1.4.803:=2)))";

// Computer in bestimmter OU
searcher.Filter = "(&(objectClass=computer)(distinguishedName=*OU=Workstations*))";
```

## 📞 Support-Informationen sammeln

### Automatisches Diagnose-Skript

```powershell
# Save as DiagnoseADComputerMover.ps1
Write-Host "=== AD Computer Mover Diagnose ===" -ForegroundColor Green

# System-Info
Write-Host "`n[System-Information]" -ForegroundColor Yellow
Get-ComputerInfo | Select-Object WindowsProductName, WindowsVersion

# Domain-Info
Write-Host "`n[Domain-Information]" -ForegroundColor Yellow
try {
    $domain = Get-ADDomain
    Write-Host "Domain: $($domain.DNSRoot)"
    Write-Host "Domain Controllers: $($domain.ReplicaDirectoryServers -join ', ')"
}
catch {
    Write-Host "Fehler beim Abrufen der Domain-Info: $($_.Exception.Message)" -ForegroundColor Red
}

# Netzwerk-Tests
Write-Host "`n[Netzwerk-Tests]" -ForegroundColor Yellow
$dcName = "dc.lps-berlin.de"
foreach ($port in @(389, 636, 3268)) {
    $result = Test-NetConnection -ComputerName $dcName -Port $port -WarningAction SilentlyContinue
    $status = if ($result.TcpTestSucceeded) { "OK" } else { "FAILED" }
    Write-Host "Port $port`: $status"
}

# PowerShell-Module
Write-Host "`n[PowerShell-Module]" -ForegroundColor Yellow
$modules = @("ActiveDirectory")
foreach ($module in $modules) {
    $installed = Get-Module -Name $module -ListAvailable
    $status = if ($installed) { "Installiert" } else { "Nicht installiert" }
    Write-Host "$module`: $status"
}

# Berechtigung
Write-Host "`n[Benutzer-Berechtigung]" -ForegroundColor Yellow
$currentUser = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name
Write-Host "Aktueller Benutzer: $currentUser"

$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
Write-Host "Administrator: $isAdmin"
```

### Support-Ticket Template

```
Betreff: AD Computer Mover - [Kurze Problembeschreibung]

=== PROBLEM-BESCHREIBUNG ===
[Detaillierte Beschreibung was nicht funktioniert]

=== FEHLERMELDUNG ===
[Exakte Fehlermeldung aus der Anwendung]

=== REPRODUKTION ===
1. [Schritt 1]
2. [Schritt 2]
3. [Fehler tritt auf]

=== SYSTEM-INFORMATION ===
- Computer: [Computer-Name]
- Benutzer: [Domain\Username]
- Windows-Version: [z.B. Windows 10 Pro 21H2]
- Anwendungs-Version: [z.B. v2.1]

=== NETZWERK ===
- Domain: lps-berlin.de
- VPN aktiv: [Ja/Nein]
- Standort: [Büro/Homeoffice]

=== BEREITS VERSUCHT ===
- [Bereits durchgeführte Lösungsversuche]

=== ANHÄNGE ===
- Screenshot des Fehlers
- Log-Dateien (falls vorhanden)
- Diagnose-Skript Ausgabe
```

## ⚠️ Bekannte Einschränkungen

### Cross-Domain Operations
- Computer zwischen verschiedenen Domains verschieben wird nicht unterstützt
- Workaround: Manuell über ADUC oder PowerShell

### Große Umgebungen
- Bei >5000 Computern kann die Anwendung langsam werden
- Empfehlung: Filtern nach spezifischen OUs

### Rights Management
- Integrierte Windows-Authentifizierung bevorzugt
- Stored Credentials werden nicht unterstützt

---

## 🔄 Eskalations-Pfad

1. **Self-Service**: Dokumentation und FAQ
2. **Level 1**: IT-Helpdesk (Standard-Probleme)
3. **Level 2**: AD-Administratoren (Berechtigung)
4. **Level 3**: Entwickler-Team (Code-Probleme)

**Notfall-Kontakt**: [24/7 IT-Hotline]

---

*Troubleshooting-Guide für AD Computer Mover v2.1*  
*Letzte Aktualisierung: August 2025*
