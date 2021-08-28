using System.Text.Json.Serialization;
using Octokit;

namespace Rexpavo.BackupManager.Classes.Models
{
    internal class Config
    {
        [JsonPropertyName("Projects")] 
        internal Project[] Projects { get; private set; }
        
    }
}