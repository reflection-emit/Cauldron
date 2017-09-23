@echo off
call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcvarsall.bat"

csc /target:exe /out:write-colored.exe /optimize+ color.cs

:: Build all projects
write-colored Cyan Building Interception Projects
for /F %%i in ('dir %~dp0..\Interception\Cauldron.Interception.Fody\*.csproj /s /b') do call:buildProject %%i
for /F %%i in ('dir %~dp0..\Interception\Cauldron.Interception.Cecilator\*.csproj /s /b') do call:buildProject %%i
write-colored Cyan Building UWP Projects
for /F %%i in ('dir %~dp0..\UWP\*.csproj /s /b') do call:buildProject %%i
write-colored Cyan Building Desktop Projects
for /F %%i in ('dir %~dp0..\Desktop\*.csproj /s /b') do call:buildProject %%i
write-colored Cyan Building Net Core Projects
for /F %%i in ('dir %~dp0..\NetCore\*.xproj /s /b') do call:buildProject %%i
write-colored Cyan Creating NuGet Packages
for /F %%i in ('dir %~dp0*.nuspec /s /b') do call:createNuget %%i

goto:eof

:: Build UWP projects
:buildProject
set projectPath=%~1
write-colored Green --------------------------
write-colored Cyan Building %projectPath%
"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.com" %projectPath% /Rebuild Release

goto:eof

:createNuget
write-colored Green --------------------------
set nuspec=%~1
write-colored Cyan Creating NuGet Packages %nuspec%
nuget.exe pack %nuspec% -OutputDir %~dp0Packages -version 1.2.18-beta
goto:eof

echo on

pause