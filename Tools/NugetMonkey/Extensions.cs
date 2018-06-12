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
            var build = version.Build;
            var revision = version.Revision + 1;
            if (revision >= 100)
            {
                build++;
                revision = 0;
            }

            return new Version(version.Major, version.Minor, build, revision);
        }
    }
}