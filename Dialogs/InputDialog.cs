using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace MoveComputerAD01.Dialogs
{
    public class InputDialog : Window
    {
        private string answer = "";
        private TextBox txtInput = new TextBox();
        private TextBlock messageText = new TextBlock();

        public string Answer => answer;

        public InputDialog()
        {
            this.Title = "Eingabe";
            this.Width = 400;
            this.Height = 150;

            // Standardnachricht
            messageText.Text = "Bitte gib etwas ein:";
            messageText.Margin = new Thickness(5);

            // Button
            Button okButton = new Button
            {
                Content = "OK",
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5)
            };

            // Layout
            StackPanel panel = new StackPanel
            {
                Children = { messageText, txtInput, okButton }
            };

            this.Content = panel;

            // Eventhandler
            okButton.Click += (s, e) =>
            {
                answer = txtInput.Text;
                this.DialogResult = true;
                this.Close();
            };
        }

        // Methode, um Nachricht zu setzen
        public void SetMessage(string message)
        {
            messageText.Text = message;
        }
    }
}
