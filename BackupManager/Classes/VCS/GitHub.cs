using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Octokit;
using Rexpavo.BackupManager.Classes.Helpers;
using Rexpavo.BackupManager.Utils;

namespace Rexpavo.BackupManager.Classes.VCS
{
    internal class GitHub
    {
        #region --Fields

        private GitHubClient _client = null;
        private User _onBehalfOfUser = null;
        private Organization _onBehalfOfOrganization = null;

        public bool ActsAsOrganization { get; private set; } = false;

        #endregion

        #region --Methods--

        internal async Task<bool> Authenticate(string token)
        {
            try
            {
                GeneralHelper.Write("Begin authentication process", GeneralHelper.eWriteTypes.Info, true);

                Credentials creds = new Credentials(token);

                GeneralHelper.Write("Authenticting...", GeneralHelper.eWriteTypes.Info, true);

                GitHubClient client = new GitHubClient(new ProductHeaderValue("rexpavo-backup-manager"));
                client.Credentials = creds;

                User currentUser = await client.User.Current();

                _onBehalfOfUser = currentUser;
                _client = client;


                GeneralHelper.Write($"Authentication succeeded! It's you {_onBehalfOfUser.Name}! :D",
                    GeneralHelper.eWriteTypes.Info, true);
                return true;
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

        internal async Task<bool> ActAsOrganization(string name)
        {
            try
            {
                GeneralHelper.Write($"Begin to try to act as {name} organization", GeneralHelper.eWriteTypes.Info,
                    true);

                Organization org = await _client.Organization.Get(name);

                _onBehalfOfOrganization = org;
                ActsAsOrganization = true;

                GeneralHelper.Write($"Now acting as {name} organization on the VCS", GeneralHelper.eWriteTypes.Info,
                    true);

                return true;
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

        internal void StopOrganizationActing()
        {
            try
            {
                GeneralHelper.Write($"Stopping to act as {_onBehalfOfOrganization.Login} organization",
                    GeneralHelper.eWriteTypes.Info, true);
                ;

                if (ActsAsOrganization)
                {
                    _onBehalfOfOrganization = null;
                    ActsAsOrganization = false;
                }

                GeneralHelper.Write($"Now performing as user {_onBehalfOfUser.Name} on the VCS",
                    GeneralHelper.eWriteTypes.Info, true);
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

        internal async Task<IReadOnlyList<Repository>> GetAllRepositories()
        {
            try
            {
                GeneralHelper.Write($"Getting all repos for {_onBehalfOfUser.Login}", GeneralHelper.eWriteTypes.Info,
                    true);

                IReadOnlyList<Repository> allForCurrent = await _client.Repository.GetAllForCurrent();

                GeneralHelper.Write($"{allForCurrent.Count} repositories retrieved", GeneralHelper.eWriteTypes.Info,
                    true);

                return allForCurrent;
            }
            catch (Exception e)
            {
                Logger.Collect(e);
                throw e;
            }
            finally
            {
                Logger.Save();
            }
        }

        internal async Task<Repository> GetRepositoryById(long id)
        {
            try
            {
                GeneralHelper.Write($"Getting repositroy with id {id}", GeneralHelper.eWriteTypes.Info, true);

                Repository requestedRepository = await _client.Repository.Get(id);

                if (requestedRepository == null)
                    GeneralHelper.Write($"Could not get repo with id {id}", GeneralHelper.eWriteTypes.Error, true);


                return requestedRepository;
            }
            catch (Exception e)
            {
                Logger.Collect(e);
                throw e;
            }
            finally
            {
                Logger.Save();
            }
        }

        internal async Task<Repository> GetRepositoryByName(string name)
        {
            try
            {
                GeneralHelper.Write($"Trying to get the {name} repository", GeneralHelper.eWriteTypes.Info, true);

                Repository requestedRepository = null;

                GeneralHelper.Write("Checking if acting as organization...", GeneralHelper.eWriteTypes.Info, true);

                if (ActsAsOrganization)
                    requestedRepository = await _client.Repository.Get(_onBehalfOfOrganization.Login, name);
                else
                    requestedRepository = await _client.Repository.Get(_onBehalfOfUser.Login, name);


                if (requestedRepository == null)
                    GeneralHelper.Write($"Could not get repo {name}", GeneralHelper.eWriteTypes.Error, true);


                return requestedRepository;
            }
            catch (Exception e)
            {
                Logger.Collect(e);
                throw e;
            }
            finally
            {
                Logger.Save();
            }
        }

        internal async Task<Branch> GetBranch(string repoName, string branchName)
        {
            try
            {
                GeneralHelper.Write($"Getting branch {branchName} from {repoName}", GeneralHelper.eWriteTypes.Info,
                    true);

                Branch requestedBranch = null;

                GeneralHelper.Write("Checking if acting as organization", GeneralHelper.eWriteTypes.Info, true);

                if (ActsAsOrganization)
                    requestedBranch =
                        await _client.Repository.Branch.Get(_onBehalfOfOrganization.Login, repoName, branchName);
                else
                    requestedBranch = await _client.Repository.Branch.Get(_onBehalfOfUser.Login, repoName, branchName);


                if (branchName == null)
                    GeneralHelper.Write("The requested branch could not be retrieved!", GeneralHelper.eWriteTypes.Error,
                        true);

                return requestedBranch;
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

        internal async Task<IReadOnlyList<Branch>> GetAllBranches(string repoName)
        {
            try
            {
                GeneralHelper.Write($"Getting all branches in {repoName}...", GeneralHelper.eWriteTypes.Info, true);

                IReadOnlyList<Branch> branches = null;
                if (ActsAsOrganization)
                    branches = await _client.Repository.Branch.GetAll(_onBehalfOfOrganization.Login, repoName);
                else
                    branches = await _client.Repository.Branch.GetAll(_onBehalfOfUser.Login, repoName);


                if (branches == null)
                    GeneralHelper.Write($"Could not retrieve branches for repo {repoName}",
                        GeneralHelper.eWriteTypes.Error,
                        true);

                return branches;
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

        internal async Task<IReadOnlyList<Organization>> GetAllOrganizations()
        {
            try
            {
                GeneralHelper.Write($"Getting all organizations for {_onBehalfOfUser.Name}",
                    GeneralHelper.eWriteTypes.Info, true);

                IReadOnlyList<Organization> orgs = await _client.Organization.GetAllForCurrent();

                if (orgs == null)
                    GeneralHelper.Write($"Could not get orgs for user {_onBehalfOfUser.Login}",
                        GeneralHelper.eWriteTypes.Error, true);


                return orgs;
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

        internal async Task<Organization> GetOrganization(string name)
        {
            try
            {
                Organization requestedOrg = await _client.Organization.Get(name);

                if (requestedOrg == null)
                    GeneralHelper.Write($"Could not get the organization {name}", GeneralHelper.eWriteTypes.Error,
                        true);

                return requestedOrg;
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


        internal bool DownloadArchive(string projectName, string branchName, string sha, string token, string savePath)
        {
            try
            {
                GeneralHelper.Write(
                    $"Bergin to download latest version of branch {branchName} of project {projectName}",
                    GeneralHelper.eWriteTypes.Info, true);

                string downloadUri = string.Empty;

                GeneralHelper.Write("Checking if acting as organization...", GeneralHelper.eWriteTypes.Info, true);
                if (ActsAsOrganization)
                    downloadUri = $"https://github.com/{_onBehalfOfOrganization.Login}/{projectName}/archive/{sha}.zip";
                else
                    downloadUri = $"https://github.com/{_onBehalfOfUser.Login}/{projectName}/archive/{sha}.zip";


                string fileLocation = Path.Combine(savePath, $"{branchName}.zip");

                using (WebClient wc = new WebClient())
                {
                    GeneralHelper.Write("Preparing webclient...", GeneralHelper.eWriteTypes.Info, true);
                    wc.Headers.Add($"Authorization: token {token}");

                    GeneralHelper.Write("Starting download...", GeneralHelper.eWriteTypes.Info, true);
                    wc.DownloadFile(downloadUri, fileLocation);
                }


                GeneralHelper.Write("Download successful!", GeneralHelper.eWriteTypes.Info, true);

                return true;
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