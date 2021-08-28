using System.Text.Json.Serialization;
using Octokit;

namespace Rexpavo.BackupManager.Classes.Models
{
    internal class Config
    {
        [JsonPropertyName("Projects")] 
        public Project[] Projects { get; set; }
        
    }
}