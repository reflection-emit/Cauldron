for %%i in (%~dp0Packages\*.nupkg) do nuget push %%i -Source https://www.nuget.org/api/v2/package
pause