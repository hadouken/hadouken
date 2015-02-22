//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

Task("Restore-NuGet-Packages")
    .Does(() =>
    {
        NuGetInstallFromConfig("./packages.config",
            new NuGetInstallSettings
            {
                ExcludeVersion = true,
                OutputDirectory = "./libs"
            });
    });

Task("Generate-CMake-Project")
    .Does(() =>
    {
        CreateDirectory("build");
        StartProcess("cmake",
            new ProcessSettings
            {
                Arguments = "../../ -DBUILD_CONFIGURATION=" + configuration,
                WorkingDirectory = "./build"
            });
    });

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Generate-CMake-Project")
    .Does(() =>
    {
        MSBuild("./build/hadouken.sln", s =>
        {
            s.Configuration = configuration;
        });
    });

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
