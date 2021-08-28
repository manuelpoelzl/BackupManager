using System;

namespace Rexpavo.BackupManager.Classes
{
    internal static class GeneralHelper
    {
        internal enum eWriteTypes
        {
            Default,
            Error,
            Info,
            Warning
        }

        internal static string[] SplitOnFirstOccurence(string text, char delimiter)
        {
            string part1 = string.Empty;
            string part2 = string.Empty;

            int index = text.IndexOf(delimiter);
            if (index > 0)
            {
                part1 = text.Substring(0, index);
                part2 = text.Substring(index + 1);
            }


            return new[] {part1, part2};
        }

        internal static void Write(string message, eWriteTypes type)
        {
            switch (type)
            {
                case eWriteTypes.Default:
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(message);
                    Console.ResetColor();

                    break;
                }
                case eWriteTypes.Error:
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(message);
                    Console.ResetColor();

                    break;
                }
                case eWriteTypes.Info:
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(message);
                    Console.ResetColor();

                    break;
                }
                case eWriteTypes.Warning:
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(message);
                    Console.ResetColor();

                    break;
                }
            }
        }
    }
}