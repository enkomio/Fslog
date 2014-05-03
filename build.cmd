@echo off

cls

"Tools\Nuget\NuGet.exe" "install" "FAKE" "-OutputDirectory" "tools" "-ExcludeVersion"
rem "tools\nuget\nuget.exe" "install" "FSharp.Formatting.CommandTool" "-OutputDirectory" "tools" "-ExcludeVersion" "-Prerelease"
rem "tools\nuget\nuget.exe" "install" "SourceLink.Fake" "-OutputDirectory" "tools" "-ExcludeVersion"

SET TARGET="Default"

IF NOT [%1]==[] (set TARGET="%1")

rem "Tools\FAKE\tools\Fake.exe" "Scripts\build.fsx" "target=%TARGET%"