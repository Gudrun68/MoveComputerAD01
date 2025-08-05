using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Principal;
using System.Diagnostics;
using MoveComputerAD01.Models;

namespace MoveComputerAD01.Services
{
    /// <summary>
    /// Service für Active Directory Operationen
    /// </summary>
    public class ActiveDirectoryService
    {
        #region Private Fields

        private readonly string _domain;
        private readonly string _username;
        private readonly string _password;
        private readonly string _ldapPath;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialisiert den AD Service mit Anmeldedaten
        /// </summary>
        /// <param name="domain">Domäne</param>
        /// <param name="username">Benutzername</param>
        /// <param name="password">Passwort</param>
        public ActiveDirectoryService(string domain, string username, string password)
        {
            _domain = domain ?? throw new ArgumentNullException(nameof(domain));
            _username = username;
            _password = password;
            
            // LDAP-Pfad aus Domäne erstellen
            _ldapPath = $"LDAP://DC={domain.Replace(".", ",DC=")}";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Testet die Verbindung zum Active Directory
        /// </summary>
        /// <returns>True wenn Verbindung erfolgreich</returns>
        public bool TestConnection()
        {
            try
            {
                using (var entry = CreateDirectoryEntry(_ldapPath))
                {
                    // Versuche auf eine Eigenschaft zuzugreifen
                    var name = entry.Name;
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lädt die AD-Struktur rekursiv
        /// </summary>
        /// <param name="path">LDAP-Pfad</param>
        /// <returns>Liste der AD-Objekte</returns>
        public List<ADObject> LoadADStructure(string path = null)
        {
            var objects = new List<ADObject>();
            var searchPath = path ?? _ldapPath;

            try
            {
                using (var entry = CreateDirectoryEntry(searchPath))
                {
                    foreach (DirectoryEntry child in entry.Children)
                    {
                        if (child.SchemaClassName == "organizationalUnit")
                        {
                            objects.Add(new ADObject
                            {
                                Name = child.Name,
                                Path = child.Path,
                                Type = ADObjectType.OrganizationalUnit,
                                Children = LoadADStructure(child.Path)
                            });
                        }
                        else if (child.SchemaClassName == "container" && 
                                child.Name.Equals("CN=Computers", StringComparison.OrdinalIgnoreCase))
                        {
                            // Standard-Container (Computers) mit Computer-Objekten laden
                            objects.Add(new ADObject
                            {
                                Name = child.Name,
                                Path = child.Path,
                                Type = ADObjectType.Container,
                                Children = LoadADStructure(child.Path)
                            });
                        }
                        else if (child.SchemaClassName == "computer")
                        {
                            objects.Add(new ADObject
                            {
                                Name = child.Name,
                                Path = child.Path,
                                Type = ADObjectType.Computer
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Fehler beim Laden der AD-Struktur: {ex.Message}", ex);
            }

            return objects;
        }

        /// <summary>
        /// Lädt nur OUs die Computer enthalten
        /// </summary>
        /// <param name="path">LDAP-Pfad</param>
        /// <returns>Liste der OUs</returns>
        public List<ADObject> LoadOUStructure(string path = null)
        {
            var objects = new List<ADObject>();
            var searchPath = path ?? _ldapPath;

            try
            {
                using (var entry = CreateDirectoryEntry(searchPath))
                {
                    foreach (DirectoryEntry child in entry.Children)
                    {
                        if (child.SchemaClassName == "organizationalUnit" && 
                            (HasComputers(child) || HasComputersRecursive(child)))
                        {
                            objects.Add(new ADObject
                            {
                                Name = child.Name,
                                Path = child.Path,
                                Type = ADObjectType.OrganizationalUnit,
                                Children = LoadOUStructure(child.Path)
                            });
                        }
                        else if (child.SchemaClassName == "container" && 
                                child.Name.Equals("CN=Computers", StringComparison.OrdinalIgnoreCase) &&
                                (HasComputers(child) || HasComputersRecursive(child)))
                        {
                            // Standard-Container (Computers) die Computer enthalten
                            objects.Add(new ADObject
                            {
                                Name = child.Name,
                                Path = child.Path,
                                Type = ADObjectType.Container,
                                Children = LoadOUStructure(child.Path)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Fehler beim Laden der OU-Struktur: {ex.Message}", ex);
            }

            return objects;
        }

        /// <summary>
        /// Verschiebt einen Computer in eine andere OU
        /// </summary>
        /// <param name="computerPath">Pfad des Computers</param>
        /// <param name="targetOUPath">Pfad der Ziel-OU</param>
        /// <returns>Erfolg der Operation</returns>
        public ComputerMoveResult MoveComputer(string computerPath, string targetOUPath)
        {
            var result = new ComputerMoveResult();

            try
            {
                using (var computer = CreateDirectoryEntry(computerPath))
                using (var targetOU = CreateDirectoryEntry(targetOUPath))
                {
                    // DN-Eigenschaften laden
                    computer.RefreshCache();
                    targetOU.RefreshCache();

                    var computerDN = computer.Properties["distinguishedName"].Value?.ToString();
                    var targetDN = targetOU.Properties["distinguishedName"].Value?.ToString();

                    result.ComputerDN = computerDN;
                    result.TargetDN = targetDN;

                    if (string.IsNullOrEmpty(computerDN) || string.IsNullOrEmpty(targetDN))
                    {
                        throw new InvalidOperationException("DN-Eigenschaften konnten nicht gelesen werden.");
                    }

                    // Prüfe ob Computer bereits in Ziel-OU ist
                    if (computerDN.Contains(targetDN))
                    {
                        result.Success = false;
                        result.Message = "Computer befindet sich bereits in der Ziel-OU.";
                        return result;
                    }

                    // Methode 1: Standard MoveTo
                    try
                    {
                        computer.MoveTo(targetOU);
                        computer.CommitChanges();
                        result.Success = true;
                        result.Message = "Computer wurde erfolgreich verschoben (Standard-Methode).";
                        result.Method = "Standard MoveTo";
                        return result;
                    }
                    catch (Exception moveEx)
                    {
                        result.Errors.Add($"Standard MoveTo: {moveEx.Message}");
                    }

                    // Methode 2: PowerShell Move-ADObject
                    try
                    {
                        var psResult = MoveComputerWithPowerShell(computerDN, targetDN);
                        if (psResult.Success)
                        {
                            result.Success = true;
                            result.Message = "Computer wurde erfolgreich verschoben (PowerShell-Methode).";
                            result.Method = "PowerShell Move-ADObject";
                            return result;
                        }
                        else
                        {
                            result.Errors.Add($"PowerShell: {psResult.Message}");
                        }
                    }
                    catch (Exception psEx)
                    {
                        result.Errors.Add($"PowerShell: {psEx.Message}");
                    }

                    // Alle Methoden fehlgeschlagen
                    result.Success = false;
                    result.Message = "Alle Verschiebungsmethoden fehlgeschlagen.";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Unerwarteter Fehler: {ex.Message}";
                result.Errors.Add(ex.Message);
                return result;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Erstellt ein DirectoryEntry mit Anmeldedaten
        /// </summary>
        private DirectoryEntry CreateDirectoryEntry(string path)
        {
            var entry = new DirectoryEntry(path);
            
            if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
            {
                entry.Username = _username;
                entry.Password = _password;
                entry.AuthenticationType = AuthenticationTypes.Secure;
            }
            
            return entry;
        }

        /// <summary>
        /// Prüft ob eine OU Computer enthält
        /// </summary>
        private bool HasComputers(DirectoryEntry ouEntry)
        {
            try
            {
                foreach (DirectoryEntry child in ouEntry.Children)
                {
                    if (child.SchemaClassName == "computer")
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Prüft rekursiv ob eine OU Computer enthält
        /// </summary>
        private bool HasComputersRecursive(DirectoryEntry ouEntry)
        {
            try
            {
                using (var searcher = new DirectorySearcher(ouEntry))
                {
                    searcher.Filter = "(objectClass=computer)";
                    searcher.SizeLimit = 1;
                    return searcher.FindOne() != null;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verschiebt Computer mit PowerShell
        /// </summary>
        private ComputerMoveResult MoveComputerWithPowerShell(string computerDN, string targetDN)
        {
            var result = new ComputerMoveResult();

            try
            {
                var psCommand = $"Move-ADObject -Identity \"{computerDN}\" -TargetPath \"{targetDN}\"";
                
                if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
                {
                    psCommand = $"$cred = New-Object System.Management.Automation.PSCredential('{_username}', (ConvertTo-SecureString '{_password}' -AsPlainText -Force)); " + psCommand + " -Credential $cred";
                }

                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"{psCommand}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    process.WaitForExit();
                    var output = process.StandardOutput.ReadToEnd();
                    var error = process.StandardError.ReadToEnd();

                    if (process.ExitCode == 0)
                    {
                        result.Success = true;
                        result.Message = "PowerShell-Verschiebung erfolgreich.";
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = $"PowerShell-Fehler: {error}";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"PowerShell-Exception: {ex.Message}";
            }

            return result;
        }

        #endregion
    }
}
