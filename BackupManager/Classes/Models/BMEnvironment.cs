using System.Text.Json.Serialization;

namespace Rexpavo.BackupManager.Classes.Models
{
    internal class BMEnvironment
    {
        [JsonPropertyName("SavePath")]
        public string SavePath { get; set; }

        [JsonPropertyName("Token")]
        public string Token { get; set; }
    }
}