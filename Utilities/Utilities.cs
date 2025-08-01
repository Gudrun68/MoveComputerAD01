using System;
using System.Security.Principal;
using System.Windows.Controls;
using MoveComputerAD01.Models;

namespace MoveComputerAD01.Utilities
{
    /// <summary>
    /// Utility-Funktionen für die Anwendung
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Prüft ob der aktuelle Benutzer Administrator-Rechte hat
        /// </summary>
        /// <returns>True wenn Administrator</returns>
        public static bool IsUserAdministrator()
        {
            try
            {
                var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gibt Informationen über den aktuellen Benutzer zurück
        /// </summary>
        /// <returns>Benutzer-Informationen</returns>
        public static string GetCurrentUserInfo()
        {
            try
            {
                var identity = WindowsIdentity.GetCurrent();
                return $"Benutzer: {identity.Name}, Typ: {identity.AuthenticationType}";
            }
            catch (Exception ex)
            {
                return $"Fehler beim Abrufen der Benutzer-Informationen: {ex.Message}";
            }
        }

        /// <summary>
        /// Bevölkert eine TreeView mit AD-Objekten
        /// </summary>
        /// <param name="treeView">Ziel TreeView</param>
        /// <param name="objects">AD-Objekte</param>
        /// <param name="showComputers">Computer anzeigen</param>
        public static void PopulateTreeView(TreeView treeView, System.Collections.Generic.List<ADObject> objects, bool showComputers = true)
        {
            if (treeView == null || objects == null)
                return;

            treeView.Items.Clear();

            foreach (var obj in objects)
            {
                var item = CreateTreeViewItem(obj, showComputers);
                if (item != null)
                    treeView.Items.Add(item);
            }
        }

        /// <summary>
        /// Erstellt ein TreeViewItem aus einem AD-Objekt
        /// </summary>
        /// <param name="adObject">AD-Objekt</param>
        /// <param name="showComputers">Computer anzeigen</param>
        /// <returns>TreeViewItem oder null</returns>
        private static TreeViewItem CreateTreeViewItem(ADObject adObject, bool showComputers)
        {
            if (adObject == null)
                return null;

            // Computer nur anzeigen wenn gewünscht
            if (adObject.Type == ADObjectType.Computer && !showComputers)
                return null;

            var item = new TreeViewItem();

            // Header je nach Objekttyp setzen
            switch (adObject.Type)
            {
                case ADObjectType.Computer:
                    item.Header = "💻 " + adObject.Name;
                    break;
                case ADObjectType.OrganizationalUnit:
                    item.Header = adObject.Name;
                    break;
                default:
                    item.Header = adObject.Name;
                    break;
            }

            item.Tag = adObject.Path;

            // Kinder hinzufügen
            if (adObject.Children != null)
            {
                foreach (var child in adObject.Children)
                {
                    var childItem = CreateTreeViewItem(child, showComputers);
                    if (childItem != null)
                        item.Items.Add(childItem);
                }
            }

            return item;
        }

        /// <summary>
        /// Formatiert eine Zeitspanne für die Anzeige
        /// </summary>
        /// <param name="timeSpan">Zeitspanne</param>
        /// <returns>Formatierte Zeitspanne</returns>
        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
                return $"{timeSpan.Days}d {timeSpan.Hours}h {timeSpan.Minutes}m";
            else if (timeSpan.TotalHours >= 1)
                return $"{timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
            else if (timeSpan.TotalMinutes >= 1)
                return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
            else
                return $"{timeSpan.Seconds}s";
        }
    }
}
