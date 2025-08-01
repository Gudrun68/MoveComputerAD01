using System.Collections.Generic;

namespace MoveComputerAD01.Models
{
    /// <summary>
    /// Typ eines AD-Objekts
    /// </summary>
    public enum ADObjectType
    {
        OrganizationalUnit,
        Computer,
        User
    }

    /// <summary>
    /// Repräsentiert ein Active Directory Objekt
    /// </summary>
    public class ADObject
    {
        /// <summary>
        /// Name des Objekts
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// LDAP-Pfad des Objekts
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Typ des Objekts
        /// </summary>
        public ADObjectType Type { get; set; }

        /// <summary>
        /// Kinder-Objekte (für OUs)
        /// </summary>
        public List<ADObject> Children { get; set; } = new List<ADObject>();

        /// <summary>
        /// Distinguished Name
        /// </summary>
        public string DistinguishedName { get; set; }
    }
}
