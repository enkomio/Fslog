@echo off

cls

"..\Tools\Nuget\NuGet.exe" "install" "FAKE" "-OutputDirectory" "..\Tools" "-ExcludeVersion"

SET TARGET="Default"

IF NOT [%1]==[] (set TARGET="%1")

cd Scripts
"..\..\Tools\FAKE\tools\Fake.exe" "build.fsx" "target=%TARGET%"
cd ..