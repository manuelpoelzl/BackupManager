using System.Collections.Generic;

namespace Rexpavo.BackupManager.Classes.Models
{
    internal class BackupResult
    {
        internal bool SavePathValid { get; set; }
        internal bool TokenProvided { get; set; }
        public bool IsUserValidated { get; set; }
        internal int NumberOfItemsToBackup { get; set; }
        internal int NumberOfActualItemsBackedUp { get; set; }
        internal int NumberOfFailedItems { get; set; }

    }
}