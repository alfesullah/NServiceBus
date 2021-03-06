﻿namespace NServiceBus.Utils
{
    using System.Linq;
    using System.Text.RegularExpressions;

    internal static class PathUtilities
    {
        public static string SanitizedPath(string commandLine)
        {
            if (commandLine.StartsWith("\""))
            {
                return (from Match match in Regex.Matches(commandLine, "\"([^\"]*)\"")
                    select match.ToString()).First().Trim('"');
            }

            return commandLine.Split(' ').First();
        }
    }
}