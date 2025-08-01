# Entwickler-Dokumentation

## 🏗️ Architektur-Übersicht

### Design-Prinzipien

Das AD Computer Mover Projekt folgt modernen Software-Design-Prinzipien:

- **Separation of Concerns**: Klare Trennung von UI, Business Logic und Datenaccess
- **Single Responsibility**: Jede Klasse hat einen klar definierten Zweck
- **Dependency Injection**: Services werden injiziert für bessere Testbarkeit
- **MVVM Pattern**: Model-View-ViewModel für saubere UI-Trennung
- **Error Handling**: Umfassende Fehlerbehandlung mit Logging

### Projekt-Struktur

```
MoveComputerAD01/
├── Utilities/           # Utility und Helper-Klassen
│   └── Utilities.cs     # Allgemeine Hilfsfunktionen
├── Dialogs/             # UI-Dialoge
│   ├── CredentialsDialog.xaml
│   ├── CredentialsDialog.xaml.cs
│   └── InputDialog.cs
├── EventHandlers/       # Event-Handler Logic
│   └── MainWindowEventHandlers.cs
├── Models/              # Datenmodelle
│   ├── ADObject.cs
│   └── ComputerMoveResult.cs
├── Services/            # Business Logic Services
│   └── ActiveDirectoryService.cs
├── ViewModels/          # MVVM ViewModels (future)
├── Views/               # UI Views (future)
├── Properties/          # Assembly-Informationen
└── docs/               # Dokumentation
```

## 🔧 Core Components

### ActiveDirectoryService.cs

**Zweck**: Zentrale Schnittstelle für alle Active Directory Operationen

**Key Methods**:
```csharp
// Verbindung testen
Task<bool> TestConnectionAsync(string domain, string username, string password)

// AD-Struktur laden
Task<List<ADObject>> LoadADStructureAsync()

// Computer verschieben
Task<ComputerMoveResult> MoveComputerAsync(string computerName, string targetOU)

// PowerShell Fallback
Task<ComputerMoveResult> MoveComputerViaPowerShellAsync(string computerName, string targetOU)
```

**Design Pattern**: 
- Repository Pattern für Datenaccess
- Strategy Pattern für verschiedene Move-Strategien
- Async/Await für non-blocking Operations

### Models/ADObject.cs

**Zweck**: Repräsentation von Active Directory Objekten

```csharp
public class ADObject
{
    public string Name { get; set; }
    public string Path { get; set; }
    public ADObjectType Type { get; set; }
    public List<ADObject> Children { get; set; }
}

public enum ADObjectType
{
    Computer,
    OrganizationalUnit,
    Container
}
```

### Models/ComputerMoveResult.cs

**Zweck**: Ergebnis von Computer-Move-Operationen

```csharp
public class ComputerMoveResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string ComputerName { get; set; }
    public string SourceOU { get; set; }
    public string TargetOU { get; set; }
    public DateTime Timestamp { get; set; }
    public Exception Exception { get; set; }
}
```

## 🔌 Dependency Management

### NuGet Packages

```xml
<PackageReference Include="System.DirectoryServices" Version="7.0.0" />
<PackageReference Include="System.DirectoryServices.AccountManagement" Version="7.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging" Version="7.0.0" />
```

### Framework Dependencies

- **.NET Framework 4.8**: Basis-Framework
- **WPF**: UI-Framework
- **System.DirectoryServices**: AD-Integration
- **PowerShell**: Fallback für AD-Operationen

## 🎯 MVVM Implementation (geplant)

### Current State: Code-Behind
Aktuell verwendet das Projekt Code-Behind im MainWindow für Event-Handling.

### Future State: MVVM
```csharp
// ViewModels/MainWindowViewModel.cs
public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IActiveDirectoryService _adService;
    
    public ObservableCollection<ADObject> Computers { get; set; }
    public ObservableCollection<ADObject> OrganizationalUnits { get; set; }
    
    public ICommand MoveComputerCommand { get; set; }
    public ICommand ConnectCommand { get; set; }
    public ICommand RefreshCommand { get; set; }
}
```

## 🧪 Testing Strategy

### Unit Tests (geplant)

```csharp
[TestClass]
public class ActiveDirectoryServiceTests
{
    private Mock<IDirectoryService> _mockDirectoryService;
    private ActiveDirectoryService _service;

    [TestInitialize]
    public void Setup()
    {
        _mockDirectoryService = new Mock<IDirectoryService>();
        _service = new ActiveDirectoryService(_mockDirectoryService.Object);
    }

    [TestMethod]
    public async Task MoveComputerAsync_ValidInput_ReturnsSuccess()
    {
        // Arrange
        var computerName = "TEST-PC";
        var targetOU = "OU=TestOU,DC=domain,DC=com";
        
        // Act
        var result = await _service.MoveComputerAsync(computerName, targetOU);
        
        // Assert
        Assert.IsTrue(result.Success);
    }
}
```

### Integration Tests

```csharp
[TestClass]
[TestCategory("Integration")]
public class ADIntegrationTests
{
    [TestMethod]
    public async Task CanConnectToTestDomain()
    {
        var service = new ActiveDirectoryService();
        var result = await service.TestConnectionAsync("test.domain", "testuser", "testpass");
        Assert.IsTrue(result);
    }
}
```

## 🚀 Build & Deployment

### MSBuild Configuration

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net48</TargetFramework>
    <UseWPF>true</UseWPF>
    <AssemblyTitle>AD Computer Mover</AssemblyTitle>
    <AssemblyVersion>2.1.0.0</AssemblyVersion>
  </PropertyGroup>
</Project>
```

### Build-Pipeline (geplant)

```yaml
# azure-pipelines.yml
trigger:
- main

pool:
  vmImage: 'windows-latest'

steps:
- task: MSBuild@1
  inputs:
    solution: '**/*.sln'
    configuration: 'Release'

- task: VSTest@2
  inputs:
    testSelector: 'testAssemblies'
    testAssemblyVer2: '**\*Tests.dll'
```

## 📝 Coding Standards

### C# Conventions

- **Naming**: PascalCase für public Members, camelCase für private
- **Documentation**: XML-Kommentare für alle public APIs
- **Async**: Alle I/O-Operationen asynchron
- **Error Handling**: Try-catch mit spezifischen Exception-Types

### XAML Conventions

```xml
<!-- Naming Convention -->
<TreeView x:Name="ADTreeViewControl" />
<Button x:Name="ConnectButton" Content="Verbinden" />

<!-- Styling -->
<Style TargetType="TreeViewItem">
    <Setter Property="IsExpanded" Value="True" />
    <Style.Triggers>
        <DataTrigger Binding="{Binding Type}" Value="Computer">
            <Setter Property="Foreground" Value="Blue" />
        </DataTrigger>
    </Style.Triggers>
</Style>
```

## 🔒 Security Considerations

### Credential Handling

```csharp
// Sichere Passwort-Verarbeitung
using (var securePassword = new SecureString())
{
    foreach (char c in password)
        securePassword.AppendChar(c);
    
    securePassword.MakeReadOnly();
    // Verwende securePassword für AD-Operationen
}
```

### LDAP Injection Prevention

```csharp
public static string EscapeLdapString(string input)
{
    if (string.IsNullOrEmpty(input))
        return input;
    
    return input.Replace("\\", "\\5c")
                .Replace("*", "\\2a")
                .Replace("(", "\\28")
                .Replace(")", "\\29")
                .Replace("\0", "\\00");
}
```

## 📊 Performance Optimizations

### Async Patterns

```csharp
// Bad: Blocking UI
var computers = service.LoadComputers();

// Good: Non-blocking
var computers = await service.LoadComputersAsync();
```

### Caching Strategy

```csharp
private readonly MemoryCache _cache = new MemoryCache();

public async Task<List<ADObject>> LoadADStructureAsync()
{
    const string cacheKey = "ADStructure";
    
    if (_cache.TryGetValue(cacheKey, out List<ADObject> cachedResult))
        return cachedResult;
    
    var result = await LoadFromAD();
    _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
    
    return result;
}
```

## 🐛 Debugging Tips

### LDAP Debugging

```csharp
// LDAP-Pfad validieren
var ldapPath = "LDAP://CN=Computer,OU=Computers,DC=domain,DC=com";
using (var entry = new DirectoryEntry(ldapPath))
{
    try
    {
        var name = entry.Name; // Triggert LDAP-Zugriff
        Console.WriteLine($"Valid LDAP path: {name}");
    }
    catch (DirectoryServicesCOMException ex)
    {
        Console.WriteLine($"LDAP Error: {ex.ExtendedError} - {ex.ExtendedErrorMessage}");
    }
}
```

### Event Tracing

```csharp
using System.Diagnostics;

[Conditional("DEBUG")]
private void TraceOperation(string operation, string details)
{
    Trace.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {operation}: {details}");
}
```

## 🔄 Future Enhancements

### Planned Features

1. **Bulk Operations**: Mehrere Computer gleichzeitig verschieben
2. **Rollback Functionality**: Änderungen rückgängig machen
3. **Audit Logging**: Detaillierte Protokollierung aller Änderungen
4. **REST API**: Web-basierte Schnittstelle
5. **PowerShell Cmdlets**: PowerShell-Integration

### Technical Debt

1. **MVVM Migration**: Von Code-Behind zu MVVM
2. **Dependency Injection**: IoC Container einführen
3. **Configuration Management**: app.config oder appsettings.json
4. **Logging Framework**: NLog oder Serilog integrieren

---

## 📞 Developer Support

**Entwickler-Community LPS-Berlin**
- **Wiki**: [Internal Wiki URL]
- **Code Reviews**: Pull Requests required
- **Architecture Decisions**: [ADR Repository]

**Code-Richtlinien**:
- Alle public APIs dokumentieren
- Unit Tests für neue Features
- Performance-Tests für kritische Pfade
- Security Reviews für AD-Operations

---

*Entwickler-Dokumentation für AD Computer Mover v2.1*  
*Letzte Aktualisierung: August 2025*
