using System.IO;
using System;

Console.WriteLine("Starting copy");

if (Args == null || Args.Count() != 4)
{
    Environment.Exit(-1);
    return;
}

var directories = Directory.GetDirectories(Args[1], Args[2] + "*");

if (directories.Length == 0)
{
    Directory.CreateDirectory(Path.Combine(Args[1], Args[2] + ".99.9.9"));
    directories = Directory.GetDirectories(Args[1], Args[2] + "*");
}

foreach (var dir in directories)
{
    foreach (var file in Directory.GetFiles(Args[0], "*.*"))
    {
        var target = Path.Combine(dir, Path.GetFileName(file));
        File.Copy(file, target, true);
        Console.WriteLine("Copied to: " + target);
    }

    var cscDir = Path.Combine(dir, "csc");
    Directory.CreateDirectory(cscDir);

    foreach (var file in Directory.GetFiles(Path.Combine(Args[3], "csc"), "*.*"))
    {
        var target = Path.Combine(cscDir, Path.GetFileName(file));
        File.Copy(file, target, true);
        Console.WriteLine("Copied to: " + target);
    }
}