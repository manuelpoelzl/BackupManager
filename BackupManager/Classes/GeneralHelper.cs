namespace Rexpavo.BackupManager.Classes
{
    internal static class GeneralHelper
    {
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
    }
}