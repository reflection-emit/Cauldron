using System;
using System.IO;
using System.Diagnostics;

if (Args == null || Args.Count() != 1)
{
    Environment.Exit(-1);
    return;
}

var fileVersion = FileVersionInfo.GetVersionInfo(Args[0]);
var newFilename = Path.Combine(Path.GetDirectoryName(Args[0]), Path.GetFileNameWithoutExtension(Args[0]) + "-" + fileVersion.FileVersion + Path.GetExtension(Args[0]));

foreach (var item in Directory.GetFiles(Path.GetDirectoryName(Args[0]), Path.GetFileNameWithoutExtension(Args[0]) + "-*"))
    File.Delete(item);

File.Move(Args[0], newFilename);