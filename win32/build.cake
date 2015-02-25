//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

Task("Clean")
    .Does(() =>
    {
        CleanDirectories(new[]
        {
            "./bin",
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
        StartProcess("cmake",
            new ProcessSettings
            {
                Arguments = "../../ -DBUILD_CONFIGURATION=" + configuration,
                WorkingDirectory = "./build"
            });
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
            "build/" + configuration);

        // Copy OpenSSL binaries.
        CopyFiles(
            GetFiles("libs/hadouken.openssl/win32/" + configuration + "/bin/*.dll"),
            "build/" + configuration);

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
            "build/" + configuration
        );
    });

Task("Create-Zip-Package")
    .IsDependentOn("Output")
    .Does(() =>
    {
        var suffix = (configuration == "Release" ? string.Empty : "-debug");
        Zip("./build/" + configuration, "bin/hadouken-win32" + suffix + ".zip");
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
                { "BinDir",       "./build/" + configuration },
                { "BuildConfiguration", configuration },
                { "BuildVersion", "0.0.0" }
            },
            Extensions = new[] { "WixFirewallExtension" },
            OutputDirectory = "./wixobj",
            ToolPath = "./libs/WiX.Toolset/tools/wix/candle.exe"
        });

        WiXLight("./wixobj/*.wixobj", new LightSettings
        {
            Extensions = new[] { "WixUtilExtension", "WixUIExtension", "WixFirewallExtension" },
            OutputFile = "./bin/hadouken-win32" + suffix + ".msi",
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
