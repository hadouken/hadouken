//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var version = File.ReadAllText("../VERSION");

Task("Clean")
    .Does(() =>
    {
        CleanDirectories(new[]
        {
            "./build/bin/" + configuration,
            "./build/" + configuration,
            "./wixobj"
        });
    });

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
        var cmakeExitCode = StartProcess("cmake",
            new ProcessSettings
            {
                Arguments = "../../",
                WorkingDirectory = "./build"
            });

        if(cmakeExitCode != 0)
        {
            throw new CakeException("CMake failed to execute.");
        }
    });

Task("Compile")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Generate-CMake-Project")
    .Does(() =>
    {
        MSBuild("./build/hadouken.sln", s =>
        {
            s.Configuration = configuration;
        });
    });

Task("Output")
    .IsDependentOn("Compile")
    .Does(() =>
    {
        // Copy Boost and libtorrent binaries.
        CopyFiles(
            GetFiles("libs/hadouken.libtorrent/win32/bin/" + configuration + "/*.dll"),
            "build/bin/" + configuration);

        // Copy OpenSSL binaries.
        CopyFiles(
            GetFiles("libs/hadouken.openssl/win32/bin/" + configuration + "/*.dll"),
            "build/bin/" + configuration);

        // Copy Poco binaries.
        var pocoSuffix = (configuration == "Release" ? string.Empty : "d");
        var pocoTemplate = "libs/hadouken.poco/win32/bin/Poco{0}{1}.dll";
        var pocoBinaries = new[]
        {
            string.Format(pocoTemplate, "Crypto", pocoSuffix),
            string.Format(pocoTemplate, "Foundation", pocoSuffix),
            string.Format(pocoTemplate, "JSON", pocoSuffix),
            string.Format(pocoTemplate, "Net", pocoSuffix),
            string.Format(pocoTemplate, "NetSSL", pocoSuffix),
            string.Format(pocoTemplate, "Util", pocoSuffix),
            string.Format(pocoTemplate, "XML", pocoSuffix)
        };

        CopyFiles(
            pocoBinaries,
            "build/bin/" + configuration
        );
    });

Task("Create-Zip-Package")
    .IsDependentOn("Output")
    .Does(() =>
    {
        var suffix = (configuration == "Release" ? string.Empty : "-debug");
        Zip("./build/bin/" + configuration, "build/bin/hadouken-" + version + "-win32" + suffix + ".zip");
    });

Task("Create-Msi-Package")
    .IsDependentOn("Output")
    .Does(() =>
    {
        var suffix = (configuration == "Release" ? string.Empty : "-debug");

        WiXCandle("./installer/*.wxs", new CandleSettings
        {
            Defines = new Dictionary<string, string>
            {
                { "BinDir",       "./build/bin/" + configuration },
                { "BuildConfiguration", configuration },
                { "BuildVersion", version }
            },
            Extensions = new[] { "WixFirewallExtension" },
            OutputDirectory = "./build/wixobj",
            ToolPath = "./libs/WiX.Toolset/tools/wix/candle.exe"
        });

        WiXLight("./build/wixobj/*.wixobj", new LightSettings
        {
            Extensions = new[] { "WixUtilExtension", "WixUIExtension", "WixFirewallExtension" },
            OutputFile = "./build/bin/hadouken-" + version + "-win32" + suffix + ".msi",
            ToolPath = "./libs/WiX.Toolset/tools/wix/light.exe"
        });
    });

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Output")
    .IsDependentOn("Create-Zip-Package")
    .IsDependentOn("Create-Msi-Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
