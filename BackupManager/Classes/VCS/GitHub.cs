using System;
using System.Collections.Generic;
using System.IO;
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

        internal async Task<IReadOnlyList<Repository>> GetAllRepositories()
        {
            try
            {
                GeneralHelper.Write($"Getting all repos for {_onBehalfOfUser.Name}", GeneralHelper.eWriteTypes.Info,
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
                Repository requestedRepository = await _client.Repository.Get(_onBehalfOfUser.Name, name);

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

        #endregion
    }
}