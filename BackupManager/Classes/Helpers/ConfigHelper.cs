using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Rexpavo.BackupManager.Classes.Models;
using Rexpavo.BackupManager.Utils;

namespace Rexpavo.BackupManager.Classes.Helpers
{
    internal class ConfigHelper
    {
        internal Config Read(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    GeneralHelper.Write("The file specified does not exist!", GeneralHelper.eWriteTypes.Error, true);
                }

                GeneralHelper.Write("Get config file content...", GeneralHelper.eWriteTypes.Info, true);
                string configFileContent = File.ReadAllText(path);
                configFileContent.Replace("\n", "").Replace("\r", "");

                GeneralHelper.Write("Config file successfully read", GeneralHelper.eWriteTypes.Info, true);

                GeneralHelper.Write("Begin parsing....", GeneralHelper.eWriteTypes.Info, true);
                Config gottenConfig = JsonSerializer.Deserialize<Config>(configFileContent);
                GeneralHelper.Write("Parsed successfully!", GeneralHelper.eWriteTypes.Info, true);

                return gottenConfig;
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
    }
}