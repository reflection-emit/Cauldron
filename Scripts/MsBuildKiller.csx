using System.Diagnostics;
using System;

foreach (var item in Process.GetProcessesByName("msbuild"))
{
    Console.WriteLine("Killing msbuild");
    item.Kill();
}