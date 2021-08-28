using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Octokit;
using Rexpavo.BackupManager.Utils;

namespace Rexpavo.BackupManager.Classes.VCS
{
    internal class GitHub
    {
        private GitHubClient _client = null;

        internal async Task<bool> Authenticate(string token)
        {
            try
            {
                Credentials creds = new Credentials(token);

                GitHubClient client = new GitHubClient(new ProductHeaderValue("rexpavo-backup-manager"));
                client.Credentials = creds;


                User currentUser = await client.User.Current();

                return true;
            }
            catch (Exception e)
            {
                
                return false;
            }
        }
    }
}