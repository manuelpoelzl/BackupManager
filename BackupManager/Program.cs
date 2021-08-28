using System;
using System.Collections.Generic;
using System.Globalization;
using Octokit;
using Rexpavo.BackupManager.Classes.Helpers;
using Rexpavo.BackupManager.Classes.Models;
using Rexpavo.BackupManager.Classes.VCS;
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

            #region --Github Auth--

            GitHub githubHelper = new GitHub();

            bool userAuthenticated = githubHelper.Authenticate(argHelper.GetArgumentValue("token")).Result;

            if (!userAuthenticated)
            {
                Logger.Collect(Logger.eMessageType.ERROR, "User could not be authenticated!");
                Logger.Save();
            }

            #endregion

            /*Actual work*/
        }
    }
}