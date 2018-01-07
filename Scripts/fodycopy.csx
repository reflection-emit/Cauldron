if (Args == null || Args.Count() != 3)
{
    Environment.Exit(-1);
    return;
}

var directories = Directory.GetDirectories(Args[1], Args[2] + "*");

if (directories.Count() == 0)
{
    Directory.CreateDirectory(Path.Combine(Args[1], Args[2] + ".99.9.9"));
    directories = Directory.GetDirectories(Args[1], Args[2] + "*");
}

foreach (var dir in directories)
    foreach (var file in Directory.GetFiles(Args[0], "*.*"))
    {
        var target = Path.Combine(dir, Path.GetFileName(file));
        File.Copy(file, target, true);
        Console.WriteLine("Copied to: " + target);
    }

