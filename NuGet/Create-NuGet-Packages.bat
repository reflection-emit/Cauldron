@echo off

C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc /target:exe /out:nugetcreator.exe /optimize+ nugetcreator.cs
nugetcreator 1.2.20-beta

echo on

pause