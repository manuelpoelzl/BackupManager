using System.Text.Json.Serialization;

namespace Rexpavo.BackupManager.Classes.Models
{
    internal class Project
    {
        [JsonPropertyName("Branches")]
        public Branch[] Branches { get; private set; }
    }
}