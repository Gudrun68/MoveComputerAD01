using System.Collections.Generic;

namespace MoveComputerAD01.Models
{
    /// <summary>
    /// Ergebnis einer Computer-Verschiebung
    /// </summary>
    public class ComputerMoveResult
    {
        /// <summary>
        /// Erfolg der Operation
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Beschreibung des Ergebnisses
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Verwendete Methode
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Computer Distinguished Name
        /// </summary>
        public string ComputerDN { get; set; }

        /// <summary>
        /// Ziel-OU Distinguished Name
        /// </summary>
        public string TargetDN { get; set; }

        /// <summary>
        /// Liste der aufgetretenen Fehler
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();
    }
}
