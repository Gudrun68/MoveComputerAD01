# Benutzerhandbuch - AD Computer Mover

## 📖 Übersicht

Der AD Computer Mover ist eine benutzerfreundliche Anwendung zur Verwaltung von Computer-Objekten in Active Directory. Mit dieser Software können Sie Computer einfach zwischen verschiedenen Organisationseinheiten (OUs) verschieben.

## 🎯 Zielgruppe

- IT-Administratoren
- Helpdesk-Mitarbeiter
- Benutzer mit AD-Verwaltungsrechten

## 🚀 Erste Schritte

### 1. Anwendung starten

1. Doppelklicken Sie auf `MoveComputerAD01.exe`
2. Die Anwendung öffnet sich und zeigt das Hauptfenster
3. Ein Anmeldedialog erscheint automatisch

### 2. Anmeldung

![Anmeldedialog](images/login-dialog.png)

**Felder ausfüllen:**
- **Domain**: `lps-berlin` (bereits vorausgefüllt)
- **Benutzername**: Ihr AD-Benutzername
- **Passwort**: Ihr AD-Passwort

**Wichtig:** Sie benötigen einen Benutzer mit Berechtigung zur Computer-Verwaltung!

### 3. Hauptfenster verstehen

Nach erfolgreicher Anmeldung sehen Sie das Hauptfenster:

```
┌─────────────────────────────────────────────────────────────┐
│ AD Computer Mover                                    [_][□][×]│
├─────────────────────────────────────────────────────────────┤
│ [Verbinden] [Computer verschieben] [Aktualisieren] [Abbrechen]│
├─────────────────┬───────────────────────────────────────────┤
│ 📁 AD-Struktur │ 🏢 Organisationseinheiten               │
│ ├─OU1          │ ├─ Computers                             │
│ ├─OU2          │ ├─ Servers                               │
│ └─💻Computer1   │ └─ Workstations                         │
│                 │                                           │
├─────────────────┴───────────────────────────────────────────┤
│ 📄 Protokoll:                                              │
│ [Log-Meldungen werden hier angezeigt...]                   │
└─────────────────────────────────────────────────────────────┘
```

#### Bereiche im Detail:

- **Linke Seite**: AD-Struktur mit Computern (💻 blau markiert)
- **Rechte Seite**: Organisationseinheiten (Ziel-OUs)
- **Unten**: Protokoll-Bereich für Status-Meldungen

## 🖱️ Computer verschieben

### Methode 1: Drag & Drop (empfohlen)

1. **Computer auswählen**: Klicken Sie auf einen Computer (💻) im linken Bereich
2. **Ziehen**: Halten Sie die Maustaste gedrückt und ziehen den Computer
3. **Ablegen**: Lassen Sie den Computer über der gewünschten OU los
4. **Bestätigen**: Ein Dialog fragt nach Bestätigung

![Drag and Drop](images/drag-drop.gif)

### Methode 2: Button-Navigation

1. **Computer auswählen**: Klicken Sie auf einen Computer im linken Bereich
2. **Ziel-OU wählen**: Klicken Sie auf die gewünschte OU im rechten Bereich
3. **Verschieben**: Klicken Sie auf "Computer verschieben"
4. **Bestätigen**: Bestätigen Sie die Aktion im Dialog

## 🔧 Funktionen im Detail

### Verbinden-Button
- **Zweck**: Neue Verbindung zum AD herstellen
- **Wann verwenden**: Bei Verbindungsproblemen oder Credential-Wechsel

### Aktualisieren-Button
- **Zweck**: AD-Struktur neu laden
- **Wann verwenden**: Nach externen Änderungen am AD

### Abbrechen-Button
- **Zweck**: Laufende Operationen stoppen
- **Wann verwenden**: Bei hängenden Vorgängen

## 📋 Protokoll verstehen

Das Protokoll zeigt wichtige Informationen:

```
✅ Erfolgreich: Computer "PC001" nach "OU=Workstations" verschoben
❌ Fehler: Berechtigung verweigert für Computer "PC002"
ℹ️ Info: Verbindung zu Domain Controller hergestellt
⚠️ Warnung: Computer "PC003" nicht gefunden
```

### Symbol-Bedeutung:
- ✅ **Erfolg**: Operation erfolgreich abgeschlossen
- ❌ **Fehler**: Operation fehlgeschlagen
- ℹ️ **Information**: Allgemeine Status-Meldung
- ⚠️ **Warnung**: Potentielles Problem

## ❗ Häufige Probleme

### Problem: "Anmeldung fehlgeschlagen"
**Lösung:**
1. Benutzername/Passwort prüfen
2. Domain-Verbindung testen
3. VPN-Verbindung prüfen (falls extern)

### Problem: "Berechtigung verweigert"
**Lösung:**
1. Mit IT-Administrator sprechen
2. Berechtigung für Computer-Verwaltung anfordern
3. Als anderer Benutzer anmelden

### Problem: "Computer nicht gefunden"
**Lösung:**
1. AD-Struktur aktualisieren (Aktualisieren-Button)
2. Computer-Name korrekt eingegeben?
3. Computer existiert im AD?

### Problem: "Verbindung zum Domain Controller fehlgeschlagen"
**Lösung:**
1. Netzwerk-Verbindung prüfen
2. Domain Controller erreichbar?
3. Firewall-Einstellungen prüfen

## ⚡ Tipps & Tricks

### Effizienter arbeiten
- **Mehrfach-Auswahl**: Strg+Klick für mehrere Computer
- **Such-Funktion**: Strg+F zum Suchen von Computern
- **Keyboard-Navigation**: Tab-Taste zum Navigieren

### Sicherheit
- **Immer bestätigen**: Prüfen Sie vor dem Verschieben die Ziel-OU
- **Backup-Strategie**: Notieren Sie ursprüngliche OU vor Änderungen
- **Test-Computer**: Testen Sie mit unwichtigen Computern

### Performance
- **Regelmäßig aktualisieren**: Halten Sie die AD-Ansicht aktuell
- **Nicht zu viele Computer**: Bei >100 Computern kann es langsam werden
- **Pausen einlegen**: Warten Sie zwischen Operationen

## 🆘 Support & Hilfe

### Selbsthilfe
1. **Protokoll prüfen**: Schauen Sie sich die Fehlermeldungen genau an
2. **Neustart**: Anwendung schließen und neu starten
3. **Aktualisieren**: AD-Struktur neu laden

### IT-Support kontaktieren
**Vor dem Anruf sammeln:**
- Screenshot des Fehlers
- Protokoll-Meldungen kopieren
- Computer-Name und Ziel-OU notieren
- Uhrzeit des Fehlers

**Kontakt:**
- **E-Mail**: it-support@lps-berlin.de
- **Telefon**: [Interne Nummer]
- **Ticket-System**: [Link zum System]

### Informationen für Support
```
Anwendung: AD Computer Mover v2.1
Benutzer: [Ihr Username]
Computer: [Ihr Computer-Name]
Fehler-Zeit: [Datum/Uhrzeit]
Fehlermeldung: [Exakte Meldung]
```

## 📚 Weiterführende Informationen

### Active Directory Grundlagen
- Was sind Organisationseinheiten (OUs)?
- Computer-Objekte in AD
- Berechtigungen und Sicherheit

### Verwandte Tools
- **Active Directory Users and Computers** (ADUC)
- **PowerShell AD-Module**
- **Group Policy Management Console**

---

## 📞 Schnellhilfe

| Problem | Lösung |
|---------|--------|
| Kann mich nicht anmelden | Benutzername/Passwort prüfen |
| Computer wird nicht angezeigt | Aktualisieren-Button drücken |
| Drag & Drop funktioniert nicht | Button-Methode verwenden |
| Anwendung hängt | Abbrechen-Button drücken |
| Keine Berechtigung | IT-Administrator kontaktieren |

---

*Für weitere Fragen wenden Sie sich an die IT-Abteilung von LPS-Berlin.*

**Version**: 2.1  
**Letzte Aktualisierung**: August 2025  
**Autor**: IT-Abteilung LPS-Berlin
