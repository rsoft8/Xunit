// <auto-generated> This file has been auto generated. </auto-generated>
//
// You can exclude this file from compilation by adding the following to your .fsproj file:
//
//   <PropertyGroup>
//     <XunitAutoGeneratedEntryPoint>false</XunitAutoGeneratedEntryPoint>
//   </PropertyGroup>

module AutoGeneratedEntryPoint

open Xunit.Runner.InProc.SystemConsole

[<EntryPoint>]
let main args =
    ConsoleRunner.Run(args).GetAwaiter().GetResult()
