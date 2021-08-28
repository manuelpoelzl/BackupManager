using System;
using System.Collections.Generic;
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
                Credentials creds = new Credentials(token);

                GitHubClient client = new GitHubClient(new ProductHeaderValue("rexpavo-backup-manager"));
                client.Credentials = creds;

                User currentUser = await client.User.Current();

                _onBehalfOfUser = currentUser;
                _client = client;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        internal async Task<bool> ActAsOrganization(string name)
        {
            try
            {
                Organization org = await _client.Organization.Get(name);

                _onBehalfOfOrganization = org;
                ActsAsOrganization = true;

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
                if (ActsAsOrganization)
                {
                    _onBehalfOfOrganization = null;
                    ActsAsOrganization = false;
                }
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
                Repository requestedRepository = null;

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
                Branch requestedBranch = null;

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
                string downloadUri = string.Empty;

                if (ActsAsOrganization)
                    downloadUri = $"https://github.com/{_onBehalfOfOrganization.Login}/{projectName}/archive/{sha}.zip";
                else
                    downloadUri = $"https://github.com/{_onBehalfOfUser.Login}/{projectName}/archive/{sha}.zip";


                string fileLocation = Path.Combine(savePath, $"{branchName}.zip");

                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add($"Authorization: token {token}");
                    wc.DownloadFile(downloadUri, fileLocation);
                }

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