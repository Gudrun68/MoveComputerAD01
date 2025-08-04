using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MoveComputerAD01.Models;
using MoveComputerAD01.Services;

namespace MoveComputerAD01.EventHandlers
{
    /// <summary>
    /// Event Handler für die MainWindow-Ereignisse
    /// </summary>
    public class MainWindowEventHandlers
    {
        #region Private Felder

        private readonly MainWindow _mainWindow;
        private readonly ActiveDirectoryService _adService;
        private TreeViewItem _draggedItem;

        #endregion

        #region Konstruktor

        /// <summary>
        /// Initialisiert die Event Handler
        /// </summary>
        /// <param name="mainWindow">Hauptfenster</param>
        /// <param name="adService">AD Service</param>
        public MainWindowEventHandlers(MainWindow mainWindow, ActiveDirectoryService adService)
        {
            _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
            _adService = adService ?? throw new ArgumentNullException(nameof(adService));
        }

        #endregion

        #region Button Event Handler

        /// <summary>
        /// Computer verschieben Button wurde geklickt
        /// </summary>
        public void MoveComputerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Validierung der Auswahl
                var selectedComputer = _mainWindow.ADTreeView.SelectedItem as TreeViewItem;
                var selectedTargetOU = _mainWindow.OuTreeView.SelectedItem as TreeViewItem;

                if (selectedComputer?.Tag == null)
                {
                    MessageBox.Show("Bitte wählen Sie einen Computer aus.", "Computer auswählen", 
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (!selectedComputer.Header.ToString().StartsWith("💻 "))
                {
                    MessageBox.Show("Das ausgewählte Objekt ist kein Computer.", "Ungültige Auswahl", 
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (selectedTargetOU?.Tag == null)
                {
                    MessageBox.Show("Bitte wählen Sie eine Ziel-OU aus.", "Ziel-OU auswählen", 
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // 2. Computer-Informationen extrahieren
                string computerName = selectedComputer.Header.ToString().Replace("💻 ", "");
                string computerPath = selectedComputer.Tag.ToString();
                string targetOUPath = selectedTargetOU.Tag.ToString();

                // 3. Benutzer-Bestätigung
                var confirmResult = MessageBox.Show(
                    $"Möchten Sie den Computer '{computerName}' wirklich verschieben?\n\n" +
                    $"Ziel-OU: {selectedTargetOU.Header}",
                    "Computer verschieben bestätigen",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirmResult != MessageBoxResult.Yes)
                {
                    _mainWindow.LogMessage("Verschiebung abgebrochen.");
                    return;
                }

                // 4. Verschiebung durchführen
                _mainWindow.LogMessage($"Starte Verschiebung von '{computerName}'...");
                _mainWindow.LogMessage($"Quell-Pfad: {computerPath}");
                _mainWindow.LogMessage($"Ziel-Pfad: {targetOUPath}");

                var result = _adService.MoveComputer(computerPath, targetOUPath);

                // 5. Ergebnis verarbeiten
                if (result.Success)
                {
                    _mainWindow.LogMessage($"✅ ERFOLG: {result.Message}");
                    _mainWindow.LogMessage($"Methode: {result.Method}");
                    
                    MessageBox.Show($"Computer '{computerName}' wurde erfolgreich verschoben!", 
                                   "Verschiebung erfolgreich", 
                                   MessageBoxButton.OK, MessageBoxImage.Information);

                    // TreeViews aktualisieren
                    _mainWindow.RefreshTreeViews();
                }
                else
                {
                    _mainWindow.LogMessage($"❌ FEHLER: {result.Message}");
                    
                    // Detaillierte Fehlermeldung
                    string detailErrors = result.Errors.Count > 0 
                        ? $"\n\nDetails:\n{string.Join("\n", result.Errors)}" 
                        : "";

                    var errorResult = MessageBox.Show(
                        $"Fehler beim Verschieben von '{computerName}':\n\n{result.Message}{detailErrors}\n\n" +
                        "Möchten Sie den PowerShell-Befehl in die Zwischenablage kopieren?",
                        "Verschiebung fehlgeschlagen",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Error);

                    if (errorResult == MessageBoxResult.Yes && !string.IsNullOrEmpty(result.ComputerDN))
                    {
                        string psCommand = $"Move-ADObject -Identity \"{result.ComputerDN}\" -TargetPath \"{result.TargetDN}\"";
                        Clipboard.SetText(psCommand);
                        _mainWindow.LogMessage("PowerShell-Befehl in Zwischenablage kopiert.");
                        
                        MessageBox.Show("PowerShell-Befehl wurde in die Zwischenablage kopiert.\n\n" +
                                       "Führen Sie ihn als Administrator in einer PowerShell mit AD-Modul aus.",
                                       "Befehl kopiert", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                _mainWindow.LogMessage($"❌ UNERWARTETER FEHLER: {ex.Message}");
                MessageBox.Show($"Unerwarteter Fehler:\n\n{ex.Message}\n\nDetails im Log verfügbar.",
                               "Unerwarteter Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Abbrechen Button wurde geklickt
        /// </summary>
        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Alle Auswahlen zurücksetzen
            if (_mainWindow.ADTreeView.SelectedItem is TreeViewItem selectedAD)
            {
                selectedAD.IsSelected = false;
            }

            if (_mainWindow.OuTreeView.SelectedItem is TreeViewItem selectedOU)
            {
                selectedOU.IsSelected = false;
            }

            _mainWindow.LogMessage("Vorgang abgebrochen - Auswahlen wurden zurückgesetzt.");
        }

        /// <summary>
        /// Hilfe Button wurde geklickt
        /// </summary>
        public void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string appDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string docsDirectory = System.IO.Path.Combine(appDirectory, "docs");
                
                // 1. Versuch: Lokale HTML-Datei
                string htmlPath = System.IO.Path.Combine(docsDirectory, "Benutzerhandbuch.html");
                if (System.IO.File.Exists(htmlPath))
                {
                    System.Diagnostics.Process.Start(htmlPath);
                    _mainWindow.LogMessage("✅ Benutzerhandbuch (HTML) geöffnet");
                    return;
                }

                // 2. Versuch: Lokale Markdown-Datei
                string mdPath = System.IO.Path.Combine(docsDirectory, "Benutzerhandbuch.md");
                if (System.IO.File.Exists(mdPath))
                {
                    System.Diagnostics.Process.Start(mdPath);
                    _mainWindow.LogMessage("✅ Benutzerhandbuch (Markdown) geöffnet");
                    return;
                }

                // 3. Versuch: Online GitHub-Repository
                string githubUrl = "https://github.com/Gudrun68/MoveComputerAD01#readme";
                System.Diagnostics.Process.Start(githubUrl);
                _mainWindow.LogMessage("✅ Online-Dokumentation geöffnet");
            }
            catch (Exception ex)
            {
                _mainWindow.LogMessage($"❌ Fehler beim Öffnen der Hilfe: {ex.Message}");
                MessageBox.Show($"Fehler beim Öffnen der Hilfe:\n\n{ex.Message}\n\n" +
                               "Weitere Informationen finden Sie unter:\n" +
                               "https://github.com/Gudrun68/MoveComputerAD01",
                               "Hilfe-Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Beenden Button wurde geklickt
        /// </summary>
        public void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.LogMessage("Anwendung wird beendet...");
            _mainWindow.Close();
        }

        #endregion

        #region Drag & Drop Event Handlers

        /// <summary>
        /// TreeView Drop Event
        /// </summary>
        public void ADTreeView_Drop(object sender, DragEventArgs e)
        {
            if (_draggedItem == null || !e.Data.GetDataPresent(DataFormats.StringFormat))
                return;

            try
            {
                string computerPath = _draggedItem.Tag as string;
                string computerName = _draggedItem.Header.ToString().Replace("💻 ", "");

                // Ziel-OU finden
                DependencyObject dep = (DependencyObject)e.OriginalSource;
                while ((dep != null) && !(dep is TreeViewItem))
                    dep = VisualTreeHelper.GetParent(dep);

                if (dep is TreeViewItem targetItem &&
                    targetItem.Tag is string targetPath &&
                    !targetItem.Header.ToString().StartsWith("💻 "))
                {
                    _mainWindow.LogMessage($"Drag & Drop: Verschiebe '{computerName}'...");
                    
                    var result = _adService.MoveComputer(computerPath, targetPath);
                    
                    if (result.Success)
                    {
                        _mainWindow.LogMessage($"✅ D&D ERFOLG: {result.Message}");
                        _mainWindow.RefreshTreeViews();
                    }
                    else
                    {
                        _mainWindow.LogMessage($"❌ D&D FEHLER: {result.Message}");
                        MessageBox.Show($"Drag & Drop Fehler: {result.Message}", "Fehler", 
                                       MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _mainWindow.LogMessage($"❌ D&D EXCEPTION: {ex.Message}");
                MessageBox.Show($"Drag & Drop Fehler: {ex.Message}", "Fehler", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _draggedItem = null;
            }
        }

        /// <summary>
        /// TreeView Mouse Down Event für Drag & Drop
        /// </summary>
        public void ADTreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            // TreeViewItem finden
            while ((dep != null) && !(dep is TreeViewItem))
                dep = VisualTreeHelper.GetParent(dep);

            if (dep is TreeViewItem item && item.Header?.ToString().StartsWith("💻 ") == true)
            {
                _draggedItem = item;
            }
        }

        /// <summary>
        /// TreeView Mouse Move Event für Drag & Drop Start
        /// </summary>
        public void ADTreeView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _draggedItem != null)
            {
                var item = _draggedItem;
                if (item?.Tag is string computerPath && item.Header.ToString().Contains("💻"))
                {
                    // Drag-Operation starten
                    var data = new DataObject(DataFormats.StringFormat, computerPath);
                    DragDrop.DoDragDrop((TreeView)sender, data, DragDropEffects.Move);
                }
            }
        }

        /// <summary>
        /// OU TreeView Drop Event
        /// </summary>
        public void OuTreeView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (!e.Data.GetDataPresent(DataFormats.StringFormat) || _draggedItem == null)
                    return;

                // Ziel-OU ermitteln
                DependencyObject dep = (DependencyObject)e.OriginalSource;
                while ((dep != null) && !(dep is TreeViewItem))
                    dep = VisualTreeHelper.GetParent(dep);

                if (dep is TreeViewItem targetItem && targetItem.Tag is string targetOuPath)
                {
                    string computerPath = _draggedItem.Tag as string;
                    string computerName = _draggedItem.Header.ToString().Replace("💻 ", "");

                    _mainWindow.LogMessage($"Drag & Drop: Verschiebe '{computerName}' nach '{targetItem.Header}'...");

                    // Verschiebung durchführen
                    var result = _adService.MoveComputer(computerPath, targetOuPath);

                    if (result.Success)
                    {
                        _mainWindow.LogMessage($"✅ ERFOLG: {result.Message}");
                        MessageBox.Show($"Computer '{computerName}' wurde erfolgreich verschoben!", 
                                       "Drag & Drop erfolgreich", 
                                       MessageBoxButton.OK, MessageBoxImage.Information);
                        _mainWindow.RefreshTreeViews();
                    }
                    else
                    {
                        _mainWindow.LogMessage($"❌ FEHLER: {result.Message}");
                        MessageBox.Show($"Drag & Drop Fehler: {result.Message}", "Fehler", 
                                       MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _mainWindow.LogMessage($"❌ Drag & Drop Fehler: {ex.Message}");
                MessageBox.Show($"Drag & Drop Fehler: {ex.Message}", "Fehler", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _draggedItem = null;
            }
        }

        /// <summary>
        /// OU TreeView Drag Over Event
        /// </summary>
        public void OuTreeView_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        #endregion
    }
}
