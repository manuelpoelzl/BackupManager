using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Octokit;
using Rexpavo.BackupManager.Classes.Helpers;
using Rexpavo.BackupManager.Classes.Models;
using Rexpavo.BackupManager.Classes.VCS;
using Rexpavo.BackupManager.Utils;
using Branch = Rexpavo.BackupManager.Classes.Models.Branch;
using Project = Rexpavo.BackupManager.Classes.Models.Project;

namespace Rexpavo.BackupManager.Classes.Core
{
    internal class BackupManager
    {
        #region --Fields--

        private Config _config = null;

        #endregion

        public BackupManager(Config config)
        {
            _config = config;
        }

        #region --Methods--

        private bool ValidateSavePathAndCreateIfNotExisting(string path = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    if (!Directory.Exists(_config.Environment.SavePath))
                    {
                        Directory.CreateDirectory(_config.Environment.SavePath);

                        GeneralHelper.Write("Specified save path did not exist - Created it",
                            GeneralHelper.eWriteTypes.Warning,
                            true);
                    }

                    return true;
                }
                else
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);

                        GeneralHelper.Write("Specified save path did not exist - Created it",
                            GeneralHelper.eWriteTypes.Warning,
                            true);
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Collect(e);
                return false;
            }
            finally
            {
                Logger.Save();
            }
        }

        private bool ValidateTokenExistence()
        {
            GeneralHelper.Write("Validating PAT...", GeneralHelper.eWriteTypes.Info, true);

            if (string.IsNullOrWhiteSpace(_config.Environment.Token))
            {
                GeneralHelper.Write("Token not provided!", GeneralHelper.eWriteTypes.Error, true);
                return false;
            }
            else
            {
                GeneralHelper.Write("Token provided!", GeneralHelper.eWriteTypes.Info, true);
                return true;
            }
        }

        private bool ValidateUser()
        {
            try
            {
                GitHub githubHelper = new GitHub();
                bool isValidated = githubHelper.Authenticate(_config.Environment.Token).Result;

                return isValidated;
            }
            catch (Exception e)
            {
                Logger.Collect(e);
                return false;
            }
            finally
            {
                Logger.Save();
            }
        }

        internal BackupResult PeformBackup()
        {
            try
            {
                GeneralHelper.Write("Starting backup...", GeneralHelper.eWriteTypes.Info, true);

                GeneralHelper.Write("Setting summary counters...", GeneralHelper.eWriteTypes.Info, true);

                int failedBackups = 0;
                int backedUp = 0;
                int totalNrToBackup = 0;

                bool isUserValidated, isSavePathValidated, isTokenExistent;

                bool[] validationGates = new[]
                    {ValidateSavePathAndCreateIfNotExisting(), ValidateTokenExistence(), ValidateUser()};

                isSavePathValidated = validationGates[0];
                isTokenExistent = validationGates[1];
                isUserValidated = validationGates[2];


                if (validationGates.Any(x => x == false))
                    throw new Exception("Validation not passed - ABORT!");

                GitHub githubHelper = new GitHub();
                githubHelper.Authenticate(_config.Environment.Token).Wait();


                foreach (Project project in _config.Projects)
                {
                    GeneralHelper.Write($"Started backup for project {project.Name}", GeneralHelper.eWriteTypes.Info,
                        true);

                    string savePath = Path.Combine(_config.Environment.SavePath,
                        DateTime.Now.ToString("dd.MM.yyyy"), project.Name);


                    if (!string.IsNullOrWhiteSpace(project.Organization))
                        githubHelper.ActAsOrganization(project.Organization).Wait();


                    foreach (Branch branch in project.Branches)
                    {
                        GeneralHelper.Write($"Performing backup for branch {branch.Name}",
                            GeneralHelper.eWriteTypes.Info, true);
                        try
                        {
                            totalNrToBackup++;

                            Octokit.Branch requestedBranch = githubHelper.GetBranch(project.Name, branch.Name).Result;

                            string lastestSHA = requestedBranch.Commit.Sha;

                            ValidateSavePathAndCreateIfNotExisting(savePath);

                            githubHelper.DownloadArchive(project.Name, branch.Name, lastestSHA,
                                _config.Environment.Token,
                                savePath);

                            backedUp++;
                        }
                        catch (Exception e)
                        {
                            failedBackups++;
                        }
                    }

                    if (githubHelper.ActsAsOrganization)
                        githubHelper.StopOrganizationActing();
                }


                GeneralHelper.Write("Backup process finished - returning summary", GeneralHelper.eWriteTypes.Info,
                    true);
                return new BackupResult()
                {
                    TokenProvided = isTokenExistent,
                    IsUserValidated = isUserValidated,
                    SavePathValid = isSavePathValidated,
                    NumberOfFailedItems = failedBackups,
                    NumberOfItemsToBackup = totalNrToBackup,
                    NumberOfActualItemsBackedUp = backedUp
                };
            }
            catch (Exception e)
            {
                Logger.Collect(e);
                throw;
            }
            finally
            {
                Logger.Save();
            }
        }

        #endregion
    }
}