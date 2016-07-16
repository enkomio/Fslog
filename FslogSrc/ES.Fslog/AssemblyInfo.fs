namespace System
open System.Reflection
open System.Runtime.InteropServices

[<assembly: AssemblyTitleAttribute("Fslog - F# Logging Framework Library")>]
[<assembly: GuidAttribute("7D77EDBF-1186-4C9C-BECB-A1F1D5305FF2")>]
[<assembly: AssemblyProductAttribute("Fslog - F# Logging Framework")>]
[<assembly: AssemblyVersionAttribute("2.0.0")>]
[<assembly: AssemblyInformationalVersionAttribute("2.0.0")>]
[<assembly: AssemblyFileVersionAttribute("2.0.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "2.0.0"
    let [<Literal>] InformationalVersion = "2.0.0"
