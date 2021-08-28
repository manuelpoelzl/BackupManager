using System;
using Rexpavo.BackupManager.Classes;

namespace Rexpavo.BackupManager
{
    class Program
    {
        static void Main(string[] args)
        {
            ArgumentHelper argumentHelper = new ArgumentHelper(args);

            bool argumementsAreValid = argumentHelper.ValidateArguments();

            if (!argumementsAreValid)
                throw new Exception("Arguments not validated!");
        }
    }
}