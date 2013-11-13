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
        |> Zip deployDir (versionDir + "Hadouken." + releaseNotes.AssemblyVersion + ".zip")
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
   ==> "All"

// Set the default target to the last node in the
// target dependency graph.
RunTargetOrDefault "All"