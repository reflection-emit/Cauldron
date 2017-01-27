@echo off
call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcvarsall.bat"

csc /target:exe /out:write-colored.exe /optimize+ color.cs

:: Build all projects
write-colored Cyan Building UWP Projects
call:buildUWPProject "Core"
call:buildUWPProject "Activator"
call:buildUWPProject "Cryptography"
call:buildUWPProject "IEnumerableExtensions"
call:buildUWPProject "Injection"
call:buildUWPProject "Localization"
call:buildUWPProject "Potions"
call:buildUWPProject "XAML"
call:buildUWPProject "XAML.Interactivity"
call:buildUWPProject "XAML.Validation"
write-colored Cyan Building Desktop Projects
call:buildDesktopProject "Core"
call:buildDesktopProject "Activator"
call:buildDesktopProject "Cryptography"
call:buildDesktopProject "Consoles"
call:buildDesktopProject "IEnumerableExtensions"
call:buildDesktopProject "Injection"
call:buildDesktopProject "Localization"
call:buildDesktopProject "Potions"
call:buildDesktopProject "XAML"
call:buildDesktopProject "XAML.Interactivity"
call:buildDesktopProject "XAML.Validation"
write-colored Cyan Creating NuGet Packages
call:createNuget "Cauldron.Core"
call:createNuget "Cauldron.Activator"
call:createNuget "Cauldron.Consoles"
call:createNuget "Cauldron.Cryptography"
call:createNuget "Cauldron.IEnumerableExtensions"
call:createNuget "Cauldron.Injection"
call:createNuget "Cauldron.Localization"
call:createNuget "Cauldron.Potions"
call:createNuget "Cauldron.XAML"
call:createNuget "Cauldron.XAML.Interactivity"
call:createNuget "Cauldron.XAML.Validation"

goto:eof

:: Build UWP projects
:buildUWPProject
for /F %%i in ('dir %~dp0..\UWP\*.%~1.csproj /s /b') do set projectPath=%%i
write-colored Green --------------------------
write-colored Cyan Building UWP %projectPath%
write-colored Yellow Platform x86
msbuild.exe %projectPath% /verbosity:m /p:Configuration=Release;Platform="x86";PreBuildEvent=;PostBuildEvent=
write-colored Yellow Platform x64
msbuild.exe %projectPath% /verbosity:m /p:Configuration=Release;Platform="x64";PreBuildEvent=;PostBuildEvent=
write-colored Yellow Platform ARM
msbuild.exe %projectPath% /verbosity:m /p:Configuration=Release;Platform="ARM";PreBuildEvent=;PostBuildEvent=

goto:eof

:: Build Desktop projects
:buildDesktopProject
for /F %%i in ('dir %~dp0..\Desktop\*.%~1.csproj /s /b') do set projectPath=%%i
write-colored Green --------------------------
write-colored Cyan Building Desktop %projectPath%

write-colored Yellow Platform Any CPU
msbuild.exe %projectPath% /verbosity:m /p:Configuration=Release;PreBuildEvent=;PostBuildEvent=

goto:eof

:createNuget
write-colored Green --------------------------
set nuspec=%~dp0%~1.nuspec
write-colored Cyan Creating NuGet Packages %nuspec%
nuget.exe pack %nuspec% -OutputDir %~dp0Packages -version 1.0.4
goto:eof

echo on