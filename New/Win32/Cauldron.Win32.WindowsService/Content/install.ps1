param($installPath, $toolsPath, $package, $project)

$rootnamespace = $project.Properties.Item("RootNamespace").Value

$item = $project.projectItems.Item("configuration.json")
$item.Properties.Item("BuildAction").Value = [int]3

$item = $project.projectItems.Item("strings.json")
$item.Properties.Item("BuildAction").Value = [int]3

$item = $project.projectItems.Item("Service.cs")
$item.Properties.Item("BuildAction").Value = [int]1
$filePath = $item.Properties.Item("FullPath").Value
(Get-Content $filePath).replace('@namespace', $rootnamespace) | Set-Content $filePath

$item = $project.projectItems.Item("ServiceMain.cs")
$item.Properties.Item("BuildAction").Value = [int]1
$filePath = $item.Properties.Item("FullPath").Value
(Get-Content $filePath).replace('@namespace', $rootnamespace) | Set-Content $filePath

$item = $project.projectItems.Item("LocaleSource.cs")
$item.Properties.Item("BuildAction").Value = [int]1
$filePath = $item.Properties.Item("FullPath").Value
(Get-Content $filePath) -replace '@namespace', $rootnamespace | Set-Content $filePath

