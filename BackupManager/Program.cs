using System;
using Rexpavo.BackupManager.Classes.Helpers;
using Rexpavo.BackupManager.Classes.Models;
using Rexpavo.BackupManager.Classes.Core;
using Rexpavo.BackupManager.Utils;

namespace Rexpavo.BackupManager
{
    class Program
    {
        static void Main(string[] args)
        {
            #region --Pre setup--

            ArgumentHelper argHelper = new ArgumentHelper(args);
            Logger.Init();

            #endregion

            #region --Argument validation

            bool argumentsAreValid = argHelper.ValidateArguments();

            if (!argumentsAreValid)
            {
                Logger.Collect(Logger.eMessageType.ERROR, "Arguments are not valid!");
                Logger.Save();
                Environment.Exit(-1);
            }

            #endregion

            #region --Read config--

            ConfigHelper configHelper = new ConfigHelper();

            Config config = configHelper.Read(argHelper.GetArgumentValue("config"));

            #endregion

            /*Actual work*/

            Classes.Core.BackupManager backupManager = new Classes.Core.BackupManager(config);

            BackupResult result = backupManager.PeformBackup();


            GeneralHelper.Write($"Total number of items to backup: {result.NumberOfItemsToBackup}",
                GeneralHelper.eWriteTypes.Info, true);
            GeneralHelper.Write($"Number of items actually backed up: {result.NumberOfActualItemsBackedUp}",
                GeneralHelper.eWriteTypes.Info, true);
            GeneralHelper.Write($"Number of items failed to backup: {result.NumberOfFailedItems}",
                GeneralHelper.eWriteTypes.Error, true);
        }
    }
}