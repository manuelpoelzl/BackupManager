using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Rexpavo.BackupManager.Utils;

namespace Rexpavo.BackupManager.Classes.Helpers
{
    internal class ArgumentHelper
    {
        public ArgumentHelper(string[] arguments)
        {
            _ArgumentCollection = arguments;
        }

        #region --Properties--

        private string[] _ArgumentCollection { get; set; }

        #endregion

        #region --Methods--

        internal bool ValidateArguments()
        {
            if (_ArgumentCollection.All(x => x.Contains(':')))
                return true;
            else
                return false;
        }

        internal string GetArgumentByName(string name)
        {
            try
            {
                string match = _ArgumentCollection.Single(x => x.ToLower().StartsWith(name.ToLower()));

                return match;
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

        internal string GetArgumentValue(string name)
        {
            try
            {
                string requiredArgument = GetArgumentByName(name);

                if (string.IsNullOrEmpty(requiredArgument))
                {
                    Logger.Collect(Logger.eMessageType.ERROR, $"Requested argument does not exists! ({name})");
                    return null;
                }

                return GeneralHelper.SplitOnFirstOccurence(requiredArgument, ':')[1];
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