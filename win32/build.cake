//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var version = File.ReadAllText("../VERSION");
var binDirectory = "./build/bin/" + configuration;
var outDirectory = "./build/out/" + configuration;

Task("Clean")
    .Does(() =>
    {
        CleanDirectories(new[]
        {
            binDirectory,
            outDirectory,
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
            binDirectory);

        // Copy OpenSSL binaries.
        CopyFiles(
            GetFiles("libs/hadouken.openssl/win32/bin/" + configuration + "/*.dll"),
            binDirectory);

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
            binDirectory
        );

        // Copy relevant dist files
        var distFiles = new []
        {
            "../dist/cacert.pem",
            "../dist/win32/hadoukend.json.template"
        };

        CopyFiles(distFiles, binDirectory);
    });

Task("Create-Zip-Package")
    .IsDependentOn("Output")
    .Does(() =>
    {
        IEnumerable<FilePath> files = GetFiles(binDirectory + "/*.dll");
        files = files.Union(GetFiles(binDirectory + "/*.exe"));
        files = files.Union(GetFiles(binDirectory + "/*.template"));

        Zip(binDirectory,
            outDirectory + "/hadouken-" + version + "-win32.zip",
            files);
    });

Task("Create-Pdb-Package")
    .IsDependentOn("Output")
    .Does(() =>
    {
        var pdbFiles = GetFiles(binDirectory + "/*.pdb");

        Zip(binDirectory,
            outDirectory + "/hadouken-" + version + "-win32.symbols.zip",
            pdbFiles);
    });

Task("Create-Msi-Package")
    .IsDependentOn("Output")
    .Does(() =>
    {
        WiXCandle("./installer/*.wxs", new CandleSettings
        {
            Defines = new Dictionary<string, string>
            {
                { "BinDir",             binDirectory },
                { "BuildConfiguration", configuration },
                { "BuildVersion",       version }
            },
            Extensions = new[] { "WixFirewallExtension" },
            OutputDirectory = "./build/wixobj",
            ToolPath = "./libs/WiX.Toolset/tools/wix/candle.exe"
        });

        WiXLight("./build/wixobj/*.wixobj", new LightSettings
        {
            Extensions = new[] { "WixUtilExtension", "WixUIExtension", "WixFirewallExtension" },
            OutputFile = outDirectory + "/hadouken-" + version + "-win32.msi",
            ToolPath = "./libs/WiX.Toolset/tools/wix/light.exe",
            RawArguments = "-spdb"
        });
    });

Task("Generate-Checksum-File")
    .IsDependentOn("Create-Zip-Package")
    .IsDependentOn("Create-Pdb-Package")
    .IsDependentOn("Create-Msi-Package")
    .Does(() =>
    {
        var checksums = new Dictionary<string, string>();

        foreach (var file in GetFiles(outDirectory + "/*.*"))
        {
            var hash = CalculateFileHash(file);
            checksums.Add(file.GetFilename().ToString(), hash.ToHex());
        }

        // Warning: poor mans JSON below. Don't hate.

        var totalChecksums = checksums.Count();
        var currentChecksum = 0;

        using(var stream = File.OpenWrite(outDirectory + "/checksums.json"))
        using(var writer = new StreamWriter(stream))
        {
            writer.WriteLine("{");

            foreach (var checksum in checksums)
            {
                currentChecksum += 1;
                var lineSeparator = ",";

                if (currentChecksum >= totalChecksums)
                {
                    lineSeparator = "";
                }

                writer.WriteLine("  \"{0}\": \"{1}\"{2}", checksum.Key, checksum.Value, lineSeparator);
            }

            writer.WriteLine("}");
        }
    });

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Output")
    .IsDependentOn("Create-Zip-Package")
    .IsDependentOn("Create-Pdb-Package")
    .IsDependentOn("Create-Msi-Package")
    .IsDependentOn("Generate-Checksum-File");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
