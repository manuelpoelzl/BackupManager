using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using Octokit;

namespace Rexpavo.BackupManager.Utils
{
    internal static class Logger
    {
        #region --Fields--

        internal enum eMessageType
        {
            INFO,
            WARNING,
            ERROR
        }

        private static readonly string _logDirectory =
            Path.Combine(Path.GetTempPath(), "Rexpavo", "BackupManager", "Logs");

        private static bool _isWired = false;
        private static StringBuilder _builder = new StringBuilder();

        #endregion

        #region --Properties--

        #endregion

        #region --Methods--

        internal static void Init()
        {
            if (_isWired)
                return;

            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);

            _isWired = true;
        }

        internal static void Collect(eMessageType type, string message, int skipFrame = 1)
        {
            string executingMethodName = new StackTrace().GetFrame(skipFrame).GetMethod().Name;
            string logTime = DateTime.Now.ToString("dd-M-yyyy--HH-mm-ss");

            string logMessage = $"{executingMethodName}|{Enum.GetName(type)}|{message}";
            _builder.AppendLine(logMessage);
        }

        internal static void Collect(Exception e)
        {
            string message = $"{e.Message} (STACK: {e.StackTrace})";

            Collect(eMessageType.ERROR, message, 2);
        }

        internal static void Save()
        {
            string fileCreateTime = DateTime.Now.ToString("dd-M-yyyy--HH");

            string fullFileName = Path.Combine(_logDirectory, $"Backup Manager Log - {fileCreateTime}");


            if (!File.Exists(fullFileName))
                File.Create(fullFileName).Dispose();

            File.AppendAllText(fullFileName, _builder.ToString());

            _builder.Clear();
        }

        #endregion
    }
}