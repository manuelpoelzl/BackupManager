using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace Rexpavo.BackupManager.Classes.Models
{
    internal class Project
    {
        [JsonPropertyName("Name")]
        public  string Name { get;  set; }
        
        [JsonPropertyName("Branches")]
        public Branch[] Branches { get;  set; }
    }
}