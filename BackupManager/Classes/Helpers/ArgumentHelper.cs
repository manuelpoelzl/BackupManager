using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
            string match = _ArgumentCollection.Single(x => x.ToLower().StartsWith(name.ToLower()));

            return match;
        }

        internal string GetArgumentValue(string name)
        {
            string requiredArgument = GetArgumentByName(name);

            if (string.IsNullOrEmpty(requiredArgument))
                return null;

            return GeneralHelper.SplitOnFirstOccurence(requiredArgument, ':')[1];
        }

        #endregion
    }
}