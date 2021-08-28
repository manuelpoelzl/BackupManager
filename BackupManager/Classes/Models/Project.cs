using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace Rexpavo.BackupManager.Classes.Models
{
    internal class Project
    {
        [JsonPropertyName("Name")]
        internal  string Name { get; private set; }
        
        [JsonPropertyName("Branches")]
        internal Branch[] Branches { get; private set; }
    }
}