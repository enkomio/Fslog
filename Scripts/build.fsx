#r @"..\..\Tools\FAKE\tools\FakeLib.dll"

open System
open System.IO
open Fake
open Fake.ReleaseNotesHelper
open Fake.AssemblyInfoFile
open Fake.Git

// propertiesl
let projectName = "FSharpLog"
let projectSummary = "Fslog - An F# logging framework with semantic approach."
let projectDescription = "Fslog is a simple yet powerful F# logging framework with semantic approach."
let authors = ["Antonio Parata"]

let release = parseReleaseNotes (File.ReadAllLines "RELEASE_NOTES.md")
let packageFiles = ["ES.Fslog.dll"]
let packages = [projectName, projectDescription]

// Directories
let baselineDirectory   = Path.GetFullPath(__SOURCE_DIRECTORY__ + @"\..\..\Artifacts\")
let buildDirectory      = baselineDirectory + @"Build\"
let nugetDirectory      = baselineDirectory + @"Nuget\"

// Tools
let nuget   = @"../../Tools/Nuget/NuGet.exe"
let xunit   = @"../../Tools/xunit-1.9.2/xunit.console.clr4.x86.exe"

let additionalFiles = ["RELEASE_NOTES.md"]

Target "RestorePackages" RestorePackages

Target "SetAssemblyInfo" (fun _ ->  
    [
        Attribute.Title "Fslog - F# Logging Framework Library"
        Attribute.Guid "7D77EDBF-1186-4C9C-BECB-A1F1D5305FF2"
        Attribute.Product "Fslog - F# Logging Framework"
        Attribute.Version release.AssemblyVersion
        Attribute.InformationalVersion release.AssemblyVersion
        Attribute.FileVersion release.AssemblyVersion
    ]
    |> CreateFSharpAssemblyInfo "../FslogSrc/ES.Fslog/AssemblyInfo.fs"
)

Target "BuildSolution" (fun _ ->
    CleanDir buildDirectory

    MSBuild buildDirectory "Build" [("Configuration", "Release")] ["../FslogSrc/FslogSrc.sln"] 
    |> Log "AppBuild-Output: "
)

Target "Test" (fun _ ->
    let dlls = !! (buildDirectory @@ "*Tests.dll")
    
    dlls
    |> xUnit (fun p -> 
        {p with
            ToolPath = xunit
        })
)

Target "CreateNuGet" (fun _ ->    
    let libDirectory = nugetDirectory + @"lib\"
    
    CleanDir nugetDirectory
    CreateDir nugetDirectory
    CreateDir libDirectory   
    
    packageFiles
    |> List.map (fun file -> buildDirectory + file)
    |> Copy libDirectory
     
    for package,description in packages do        
        NuGet (fun p ->
            {p with
                Authors = authors
                Project = package
                Description = description
                Version = release.NugetVersion
                OutputPath = nugetDirectory
                WorkingDir = nugetDirectory
                Summary = projectSummary
                Title = projectName
                ReleaseNotes = release.Notes |> toLines
                Dependencies = p.Dependencies
                AccessKey = getBuildParamOrDefault "nugetkey" ""
                Publish = hasBuildParam "nugetkey"
                ToolPath = nuget  }) "fslog.nuspec"
)


Target "Default" DoNothing

// Dependencies
"RestorePackages"
    ==> "SetAssemblyInfo"
    ==> "BuildSolution"   
    ==> "Test"  
    ==> "CreateNuGet"
    ==> "Default"    

// start build
RunTargetOrDefault "Default"