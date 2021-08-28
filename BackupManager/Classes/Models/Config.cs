using System.Text.Json.Serialization;
using Octokit;

namespace Rexpavo.BackupManager.Classes.Models
{
    internal class Config
    {
        [JsonPropertyName("Environment")]
        public BMEnvironment Environment { get; set; }

        [JsonPropertyName("Projects")]
        public Project[] Projects { get; set; }
    }
}