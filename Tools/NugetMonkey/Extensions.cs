using Cauldron;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace NugetMonkey
{
    internal static class Extensions
    {
        public static bool Contains(this XmlElement element, string key)
        {
            try
            {
                var result = element[key];
                return result != null;
            }
            catch
            {
                return false;
            }
        }

        public static IEnumerable<XmlElement> Each(this XmlElement xmlElement)
        {
            foreach (var item in xmlElement)
                yield return item as XmlElement;
        }

        public static string EnclosedIn(this string target, string start, string end)
        {
            if (string.IsNullOrEmpty(target))
                return null;

            int startPos = target.IndexOf(start);

            if (startPos < 0)
                return null;

            int endPos = target.IndexOf(end, startPos + 1);

            if (endPos <= startPos)
                return null;

            return target.Substring(startPos, endPos - startPos + end.Length);
        }

        public static string GetStackTrace(this Exception e)
        {
            var sb = new StringBuilder();
            var ex = e;

            do
            {
                sb.AppendLine("Exception Type: " + ex.GetType().Name);
                sb.AppendLine("Source: " + ex.Source);
                sb.AppendLine(ex.Message);
                sb.AppendLine("------------------------");
                sb.AppendLine(ex.StackTrace);
                sb.AppendLine("------------------------");

                ex = ex.InnerException;
            } while (ex != null);

            return sb.ToString();
        }

        public static Version Increment(this Version version)
        {
            var defaultVersion =  NugetMonkeyJson.BasicVersion.SplitVersion();

            var major = version.Major > defaultVersion.Item1? version.Major : defaultVersion.Item1;
            var minor = version.Minor > defaultVersion.Item2? version.Minor : defaultVersion.Item2;
            var revision = defaultVersion.Item3 < 0 ? 0 : version.Revision + 1;

            if (revision >= 100)
            {
                minor++;
                revision = 0;
            }

            return new Version(major, minor, 0, revision);
        }

        public static Tuple<int, int, int, bool> SplitVersion(this string versionString)
        {
            var splitted = versionString.Split('.');

            var major = "1";
            var minor = "0";
            var revision = "0";
            var isBeta = false;

            if (splitted.Length == 3)
            {
                major = splitted[0];
                minor = splitted[1];

                if (splitted[2].IndexOf('-') >= 0)
                {
                    var revisionBeta = splitted[2].Split('-');
                    revision = revisionBeta[0];
                    isBeta = revisionBeta[1].ToBool();
                }
                else
                    revision = splitted[2];
            }

            return new Tuple<int, int, int, bool>(major.ToInteger(), minor.ToInteger(), revision.ToInteger(), isBeta);
        }
    }
}