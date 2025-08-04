using System;
using System.Windows;
using System.Windows.Controls;
using MoveComputerAD01.Services;
using MoveComputerAD01.EventHandlers;
using MoveComputerAD01.Utilities;
using MoveComputerAD01.Dialogs;

namespace MoveComputerAD01
{
    /// <summary>
    /// Hauptfenster der AD Computer Mover Anwendung
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Fields

        private ActiveDirectoryService _adService;
        private MainWindowEventHandlers _eventHandlers;

        #endregion

        #region Public Properties

        /// <summary>
        /// TreeView für AD-Struktur (öffentlich für Event Handler)
        /// </summary>
        public TreeView ADTreeView => ADTreeViewControl;

        /// <summary>
        /// TreeView für OU-Struktur (öffentlich für Event Handler)
        /// </summary>
        public TreeView OuTreeView => OuTreeViewControl;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialisiert das Hauptfenster
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            
            // Fenster anzeigen und dann initialisieren
            this.Loaded += MainWindow_Loaded;
        }

        /// <summary>
        /// Wird ausgeführt wenn das Fenster geladen ist
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeApplication();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialisiert die Anwendung
        /// </summary>
        private void InitializeApplication()
        {
            try
            {
                // Benutzer-Informationen loggen
                LogMessage("=== AD Computer Mover gestartet ===");
                LogMessage(MoveComputerAD01.Utilities.Utilities.GetCurrentUserInfo());
                LogMessage($"Administrator-Rechte: {(MoveComputerAD01.Utilities.Utilities.IsUserAdministrator() ? "Ja" : "Nein")}");

                if (!MoveComputerAD01.Utilities.Utilities.IsUserAdministrator())
                {
                    LogMessage("⚠️ WARNUNG: Anwendung läuft nicht als Administrator.");
                    LogMessage("Dies kann zu Berechtigungsproblemen beim Verschieben führen.");
                }

                // AD-Anmeldedaten abfragen
                if (!RequestCredentials())
                {
                    LogMessage("Anwendung wird beendet - keine Anmeldedaten.");
                    Application.Current.Shutdown();
                    return;
                }

                // TreeViews laden
                LoadTreeViews();

                LogMessage("=== Anwendung erfolgreich initialisiert ===");
            }
            catch (Exception ex)
            {
                LogMessage($"❌ FEHLER bei der Initialisierung: {ex.Message}");
                MessageBox.Show($"Fehler beim Starten der Anwendung:\n\n{ex.Message}", 
                               "Initialisierungsfehler", 
                               MessageBoxButton.OK, 
                               MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Fragt AD-Anmeldedaten ab
        /// </summary>
        /// <returns>True wenn Anmeldedaten erfolgreich abgefragt</returns>
        private bool RequestCredentials()
        {
            var credDialog = new CredentialsDialog();
            // Owner erst setzen nachdem das Hauptfenster angezeigt wurde
            if (this.IsLoaded)
            {
                credDialog.Owner = this;
            }

            if (credDialog.ShowDialog() == true)
            {
                try
                {
                    // AD Service erstellen
                    _adService = new ActiveDirectoryService(
                        credDialog.Domain, 
                        credDialog.Username, 
                        credDialog.Password);

                    // Verbindung testen
                    LogMessage("Teste AD-Verbindung...");
                    if (_adService.TestConnection())
                    {
                        LogMessage("✅ AD-Verbindung erfolgreich.");
                        
                        // Event Handlers initialisieren
                        _eventHandlers = new MainWindowEventHandlers(this, _adService);
                        
                        return true;
                    }
                    else
                    {
                        LogMessage("❌ AD-Verbindung fehlgeschlagen.");
                        MessageBox.Show("Verbindung zum Active Directory fehlgeschlagen.\n\n" +
                                       "Bitte prüfen Sie Ihre Anmeldedaten und die Netzwerkverbindung.",
                                       "Verbindungsfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"❌ FEHLER beim Erstellen der AD-Verbindung: {ex.Message}");
                    MessageBox.Show($"Fehler beim Verbinden mit Active Directory:\n\n{ex.Message}",
                                   "Verbindungsfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return false;
        }

        #endregion

        #region TreeView Management

        /// <summary>
        /// Lädt beide TreeViews
        /// </summary>
        private void LoadTreeViews()
        {
            try
            {
                LogMessage("Lade AD-Struktur...");
                
                // AD-Struktur laden (mit Computern)
                var adStructure = _adService.LoadADStructure();
                MoveComputerAD01.Utilities.Utilities.PopulateTreeView(ADTreeViewControl, adStructure, showComputers: true);
                
                LogMessage($"AD-Struktur geladen: {adStructure.Count} Objekte");

                // OU-Struktur laden (nur OUs mit Computern)
                var ouStructure = _adService.LoadOUStructure();
                MoveComputerAD01.Utilities.Utilities.PopulateTreeView(OuTreeViewControl, ouStructure, showComputers: false);
                
                LogMessage($"OU-Struktur geladen: {ouStructure.Count} OUs");
            }
            catch (Exception ex)
            {
                LogMessage($"❌ FEHLER beim Laden der TreeViews: {ex.Message}");
                MessageBox.Show($"Fehler beim Laden der AD-Struktur:\n\n{ex.Message}",
                               "Ladefehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Aktualisiert beide TreeViews
        /// </summary>
        public void RefreshTreeViews()
        {
            try
            {
                LogMessage("Aktualisiere TreeViews...");
                LoadTreeViews();
                LogMessage("TreeViews erfolgreich aktualisiert.");
            }
            catch (Exception ex)
            {
                LogMessage($"❌ FEHLER beim Aktualisieren der TreeViews: {ex.Message}");
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Computer verschieben Button Click
        /// </summary>
        private void MoveComputerButton_Click(object sender, RoutedEventArgs e)
        {
            _eventHandlers?.MoveComputerButton_Click(sender, e);
        }

        /// <summary>
        /// Abbrechen Button Click
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _eventHandlers?.CancelButton_Click(sender, e);
        }

        /// <summary>
        /// TreeView Drop Event
        /// </summary>
        private void ADTreeView_Drop(object sender, System.Windows.DragEventArgs e)
        {
            _eventHandlers?.ADTreeView_Drop(sender, e);
        }

        /// <summary>
        /// TreeView Mouse Down Event
        /// </summary>
        private void ADTreeView_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _eventHandlers?.ADTreeView_PreviewMouseLeftButtonDown(sender, e);
        }

        #endregion

        #region Logging

        /// <summary>
        /// Fügt eine Nachricht zum Log hinzu
        /// </summary>
        /// <param name="message">Log-Nachricht</param>
        public void LogMessage(string message)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    LogTextBox.AppendText($"{DateTime.Now:HH:mm:ss}: {message}\n");
                    LogTextBox.ScrollToEnd();
                });
            }
            catch
            {
                // Logging-Fehler ignorieren
            }
        }

        /// <summary>
        /// Event-Handler für den Beenden-Button
        /// </summary>
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogMessage("Anwendung wird beendet...");
                
                // Sauberes Schließen der Anwendung
                this.Close();
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Beenden: {ex.Message}");
                // Auch bei Fehler die Anwendung schließen
                Application.Current.Shutdown();
            }
        }

        #endregion
    }
}