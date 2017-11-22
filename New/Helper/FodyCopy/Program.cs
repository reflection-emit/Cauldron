using System;
using System.IO;

namespace FodyCopy
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args == null || args.Length != 3)
            {
                Environment.Exit(-1);
                return;
            }

            var directories = Directory.GetDirectories(args[1], args[2] + "*");

            if (directories.Length == 0)
            {
                Directory.CreateDirectory(Path.Combine(args[1], args[2] + ".99.9.9"));
                directories = Directory.GetDirectories(args[1], args[2] + "*");
            }

            foreach (var dir in directories)
                foreach (var file in Directory.GetFiles(args[0], "*.*"))
                    File.Copy(file, Path.Combine(dir, Path.GetFileName(file)), true);
        }
    }
}