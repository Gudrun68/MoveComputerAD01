using System.Windows;

namespace MoveComputerAD01.Dialogs
{
    /// <summary>
    /// Dialog für die Eingabe von Active Directory Anmeldedaten
    /// </summary>
    public partial class CredentialsDialog : Window
    {
        #region Properties

        /// <summary>
        /// Eingegebene Domäne
        /// </summary>
        public string Domain { get; private set; }

        /// <summary>
        /// Eingegebener Benutzername
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Eingegebenes Passwort
        /// </summary>
        public string Password { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialisiert einen neuen CredentialsDialog
        /// </summary>
        public CredentialsDialog()
        {
            InitializeComponent();
            
            // Fokus auf Benutzername setzen
            UsernameTextBox.Focus();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// OK-Button wurde geklickt
        /// </summary>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Validierung
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                MessageBox.Show("Bitte geben Sie einen Benutzernamen ein.", "Eingabe erforderlich", 
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                UsernameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                MessageBox.Show("Bitte geben Sie ein Passwort ein.", "Eingabe erforderlich", 
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordBox.Focus();
                return;
            }

            // Werte übernehmen
            Domain = DomainTextBox.Text.Trim();
            Username = UsernameTextBox.Text.Trim();
            Password = PasswordBox.Password;

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Abbrechen-Button wurde geklickt
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #endregion
    }
}
