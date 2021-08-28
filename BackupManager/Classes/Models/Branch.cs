using System.Text.Json.Serialization;

namespace Rexpavo.BackupManager.Classes.Models
{
    internal class Branch
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }
        
    }
}