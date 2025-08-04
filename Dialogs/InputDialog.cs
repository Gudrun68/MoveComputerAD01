using System.Windows;
using System.Windows.Controls;

namespace MoveComputerAD01.Dialogs
{
    /// <summary>
    /// Einfacher Eingabedialog für Textabfragen
    /// </summary>
    public class InputDialog : Window
    {
        #region Private Felder

        private string answer = "";
        private readonly TextBox txtInput = new TextBox();
        private readonly TextBlock messageText = new TextBlock();

        #endregion

        #region Eigenschaften

        /// <summary>
        /// Die eingegebene Antwort
        /// </summary>
        public string Answer => answer;

        #endregion

        #region Konstruktor

        /// <summary>
        /// Initialisiert einen neuen InputDialog
        /// </summary>
        public InputDialog()
        {
            InitializeDialog();
        }

        #endregion

        #region Private Methoden

        /// <summary>
        /// Initialisiert den Dialog
        /// </summary>
        private void InitializeDialog()
        {
            this.Title = "Eingabe";
            this.Width = 400;
            this.Height = 180;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.ResizeMode = ResizeMode.NoResize;

            // Nachrichtentext
            messageText.Text = "Bitte geben Sie einen Wert ein:";
            messageText.Margin = new Thickness(10, 10, 10, 5);

            // Eingabefeld
            txtInput.Margin = new Thickness(10, 5, 10, 10);

            // Buttons
            var okButton = new Button
            {
                Content = "OK",
                Width = 80,
                Height = 25,
                Margin = new Thickness(5),
                IsDefault = true
            };

            var cancelButton = new Button
            {
                Content = "Abbrechen",
                Width = 80,
                Height = 25,
                Margin = new Thickness(5),
                IsCancel = true
            };

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10),
                Children = { cancelButton, okButton }
            };

            // Hauptlayout
            var mainPanel = new StackPanel
            {
                Children = { messageText, txtInput, buttonPanel }
            };

            this.Content = mainPanel;

            // Event Handler
            okButton.Click += (s, e) =>
            {
                answer = txtInput.Text?.Trim() ?? "";
                this.DialogResult = true;
                this.Close();
            };

            cancelButton.Click += (s, e) =>
            {
                this.DialogResult = false;
                this.Close();
            };

            // Fokus auf Eingabefeld
            this.Loaded += (s, e) => txtInput.Focus();
        }

        #endregion

        #region Öffentliche Methoden

        /// <summary>
        /// Setzt den Nachrichtentext
        /// </summary>
        /// <param name="message">Nachricht</param>
        public void SetMessage(string message)
        {
            messageText.Text = message ?? "Bitte geben Sie einen Wert ein:";
        }

        /// <summary>
        /// Setzt den Standardwert
        /// </summary>
        /// <param name="defaultValue">Standardwert</param>
        public void SetDefaultValue(string defaultValue)
        {
            txtInput.Text = defaultValue ?? "";
        }

        #endregion
    }
}
