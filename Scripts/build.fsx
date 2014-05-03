#r @"..\Tools\FAKE\tools\FakeLib.dll"

open System.IO
open Fake
open Fake.Git
open Fake.FSharpFormatting
open Fake.ReleaseNotesHelper
open Fake.AssemblyInfoFile

// properties
let projectName = "Fslog"
let projectSummary = "Fslog - A F# logging framework with semantic approach."
let projectDescription = "Fslog is simple yet powerful F# logging framework with semantic approach."
let authors = ["Antonio Parata"]
let mail = "aparata@gmail.com"

let release = parseReleaseNotes (File.ReadAllLines "RELEASE_NOTES.md")

let packages = [projectName, projectDescription]

let buildDir = "./build"
let deployDir = "./Publish"
let nugetDir = "./nuget"
let deployZip = deployDir @@ sprintf "%s-%s.zip" projectName release.AssemblyVersion
let packagesDir = "./packages"

let additionalFiles = ["RELEASE_NOTES.md"]

// Targets
Target "Clean" (fun _ -> CleanDirs [buildDir; deployDir; nugetDir])

Target "RestorePackages" RestorePackages

Target "SetAssemblyInfo" (fun _ ->
    let common = [
         Attribute.Product "Fslog - F# Logging Framework"
         Attribute.Version release.AssemblyVersion
         Attribute.InformationalVersion release.AssemblyVersion
         Attribute.FileVersion release.AssemblyVersion]

    [Attribute.Title "Fslog - F# Logging Framework Library"
     Attribute.Guid "7D77EDBF-1186-4C9C-BECB-A1F1D5305FF2"] @ common
    |> CreateFSharpAssemblyInfo "../FslogSrc/ES.Fslog/AssemblyInfo.fs"
)

Target "BuildSolution" (fun _ ->
    MSBuildWithDefaults "Build" ["../FslogSrc/FslogSrc.sln"]
    |> Log "AppBuild-Output: "
)

Target "CreateNuGet" (fun _ ->
    for package,description in packages do
        (*let nugetDocsDir = nugetDir @@ "docs"
        let nugetToolsDir = nugetDir @@ "tools"

        CleanDir nugetDocsDir
        CleanDir nugetToolsDir
        
        match package with
        | p when p = projectName ->
            !! (buildDir @@ "**/*.*") |> Copy nugetToolsDir
            CopyDir nugetToolsDir @"./lib/fsi" allFiles                   
        | _ ->
            CopyDir nugetToolsDir (buildDir @@ package) allFiles
            CopyTo nugetToolsDir additionalFiles
        !! (nugetToolsDir @@ "*.pdb") |> DeleteFiles*)

        NuGet (fun p ->
            {p with
                Authors = authors
                Project = package
                Description = description
                Version = release.NugetVersion
                OutputPath = nugetDir
                Summary = projectSummary
                ReleaseNotes = release.Notes |> toLines
                Dependencies =
                    if package <> "FAKE.Core" && package <> projectName then
                      ["FAKE.Core", RequireExactly (NormalizeVersion release.AssemblyVersion)]
                    else p.Dependencies
                AccessKey = getBuildParamOrDefault "nugetkey" ""
                Publish = hasBuildParam "nugetkey"
                ToolPath = "./tools/NuGet/nuget.exe"  }) "fake.nuspec"
)

Target "Release" (fun _ ->
    StageAll ""
    Commit "" (sprintf "Bump version to %s" release.NugetVersion)
    Branches.push ""

    Branches.tag "" release.NugetVersion
    Branches.pushTag "" "origin" release.NugetVersion
)

Target "Default" DoNothing

// Dependencies
"Clean"
    ==> "RestorePackages"
    ==> "SetAssemblyInfo"
    ==> "BuildSolution"
    ==> "Test"    
    ==> "Default"
    ==> "CopyLicense"
    =?> ("GenerateDocs", isLocalBuild && not isLinux)
    =?> ("SourceLink", isLocalBuild && not isLinux)
    =?> ("CreateNuGet", not isLinux)
    =?> ("ReleaseDocs", isLocalBuild && not isLinux)
    ==> "Release"

// start build
RunTargetOrDefault "Default"