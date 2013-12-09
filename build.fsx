// Load the FAKE assembly.
#r @"tools/FAKE.Core/tools/FakeLib.dll"
open Fake 

// Get the release notes.
// It's here we will get stuff like version number from.
let releaseNotes = 
    ReadFile "ReleaseNotes.md"
    |> ReleaseNotesHelper.parseReleaseNotes

// Set the build mode (default to release).
let buildMode = getBuildParamOrDefault "buildMode" "Release"

// Define directories.
let outputDir = "./src/Main/Hadouken.Service/bin/" + buildMode + "/"
let buildResultDir = "./build/"
let versionDir = buildResultDir @@ releaseNotes.AssemblyVersion + "/"
let deployDir = versionDir @@ "bin/"
let testResultsDir = versionDir @@ "test-results/"

Target "Clean" (fun _ ->

    trace "\n----------------------------------------"
    trace "CLEANING DIRECTORIES"
    trace "----------------------------------------\n"

    CleanDirs [versionDir; deployDir; testResultsDir]
)

Target "Build" (fun _ ->

    trace "\n----------------------------------------"
    trace "BUILDING SOLUTION"
    trace "----------------------------------------\n"

    MSBuild null "Build" ["Configuration", buildMode] ["./Hadouken.sln"]
    |> Log "AppBuild-Output: "
)

Target "Tests" (fun _ ->

    trace "\n----------------------------------------"
    trace "RUNNING UNIT TESTS"
    trace "----------------------------------------\n"

    !! ("./src/**/bin/Release/*.Tests.dll")
        |> NUnit (fun p -> 
            {p with
                DisableShadowCopy = true; 
                OutputFile = testResultsDir + "TestResults.xml"})
)

Target "Copy" (fun _ ->

    trace "\n----------------------------------------"
    trace "COPYING FILES"
    trace "----------------------------------------\n"

    CopyFile deployDir "./LICENSE"
    CopyFile deployDir "./README.md"
    CopyFile deployDir "./ReleaseNotes.md"    

    let allFiles (path:string) = true
    CopyDir deployDir outputDir allFiles
)

Target "Zip" (fun _ ->

    trace "\n----------------------------------------"
    trace "PACKAGING FILES"
    trace "----------------------------------------\n"

    !! (deployDir + "**/*") 
        |> Zip deployDir (versionDir + "hdkn-" + releaseNotes.AssemblyVersion + ".zip")
)

Target "MSI" (fun _ ->
    trace "\n----------------------------------------"
    trace "BUILDING MSI PACKAGE"
    trace "----------------------------------------\n"

    let candleSources =
        [
            "src/Installer/Hadouken.wxs";
            "src/Installer/Components/Config.wxs";
            "src/Installer/Components/Core.wxs";
            "src/Installer/Components/Lib.wxs";
            "src/Installer/Components/Plugins.wxs";
            "src/Installer/Components/Service.wxs"
        ]
        |> String.concat " "

    let binDir = "build/" + releaseNotes.AssemblyVersion + "/bin"

    let candleResult =
        ExecProcess (fun info -> 
            info.FileName <- "./tools/wix-3.8/candle.exe"
            info.Arguments <- "-ext WixUIExtension -ext WixNetFxExtension -out src/Installer/obj/ -dBinDir=" + binDir + " -dBuildVersion=" + releaseNotes.AssemblyVersion + " " + candleSources
        ) (System.TimeSpan.FromMinutes 1.)
 
    if candleResult <> 0 then failwith "MSI (Candle) failed."

    let lightSources =
        [
            "src/Installer/obj/Config.wixobj";
            "src/Installer/obj/Core.wixobj";
            "src/Installer/obj/Hadouken.wixobj";
            "src/Installer/obj/Lib.wixobj";
            "src/Installer/obj/Plugins.wixobj";
            "src/Installer/obj/Service.wixobj"
        ]
        |> String.concat " "

    let msiPath = "build/" + releaseNotes.AssemblyVersion + "/hdkn-" + releaseNotes.AssemblyVersion + ".msi"

    let lightResult =
        ExecProcess (fun info ->
            info.FileName <- "./tools/wix-3.8/light.exe"
            info.Arguments <- "-ext WixUIExtension -ext WixNetFxExtension -pdbout src/Installer/pdb/Hadouken.wixpdb -out " + msiPath + " " + lightSources
        ) (System.TimeSpan.FromMinutes 1.)

    if lightResult <> 0 then failwith "MSI (Light) failed."
)

Target "Help" (fun _ ->
    printfn ""
    printfn "  Please specify the target by calling 'build <Target>'"
    printfn "  Targets for building:"
    printfn ""
    printfn "  * Clean"
    printfn "  * Build"
    printfn "  * Tests"
    printfn "  * All (calls all previous)"
    printfn "")

Target "All" DoNothing

// Setup the target dependency graph.
"Clean"
   ==> "Build"
   ==> "Tests"
   ==> "Copy"
   ==> "Zip"
   ==> "MSI"
   ==> "All"

// Set the default target to the last node in the
// target dependency graph.
RunTargetOrDefault "All"