#r "System.Web.Extensions.dll"

/////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var Target        = Argument("target", "Default");
var Configuration = Argument("configuration", "Release");

// VERSION INFORMATION
var BuildNumber   = EnvironmentVariable("BUILD_NUMBER");
var Version       = System.IO.File.ReadAllText("../VERSION");
var VersionSuffix = "";

// VARIOUS DIRECTORIES
var BinariesDirectory = "./build/bin/" + Configuration;
var OutputDirectory   = "./build/out/" + Configuration;

// GIT SPECIFIC VARIABLES
var GitHubToken      = EnvironmentVariable("GITHUB_TOKEN");
var GitBranch        = EnvironmentVariable("BUILD_VCS_BRANCH");
var GitCommitish     = EnvironmentVariable("BUILD_VCS_NUMBER");
var GitHubRepository = "hadouken/hadouken";

// CHOCOLATEY
var ChocolateyApiKey = EnvironmentVariable("CHOCOLATEY_API_KEY");

public string RunCommand(string executable, string args)
{
    IEnumerable<string> output;

    var exitCode = StartProcess(executable,
        new ProcessSettings
        {
            Arguments = args,
            RedirectStandardOutput = true
        },
        out output);

    if (exitCode == 0)
    {
        return output.First();
    }

    return string.Empty;
}

public string ToJson(object obj)
{
    return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(obj);
}

public T FromJson<T>(string json)
{
    return new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<T>(json);
}

Task("Clean")
    .Does(() =>
    {
        CleanDirectories(new[]
        {
            BinariesDirectory,
            BinariesDirectory + "/js/plugins",
            BinariesDirectory + "/js/rpc",
            OutputDirectory,
            "./build/" + Configuration,
            "./build/wixobj"
        });
    });

Task("Prepare-Version-Suffix")
    .WithCriteria(() => GitBranch != "master")
    .Does(() =>
    {
        if(string.IsNullOrEmpty(GitBranch))
        {
            GitBranch = RunCommand("git", "rev-parse --abbrev-ref HEAD");        
        }

        if (GitBranch == "master") return;

        if (!string.IsNullOrEmpty(BuildNumber))
        {
            VersionSuffix = "-build" + BuildNumber;
        }
        else
        {
            VersionSuffix = "-local";
        }
    });

Task("Get-Git-Revision")
    .WithCriteria(() => string.IsNullOrEmpty(GitCommitish))
    .OnError(exception => { GitCommitish = "<norev>"; })
    .Does(() =>
    {
        GitCommitish = RunCommand("git", "log -1 --format=%H") ?? "<norev>";
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
                Arguments = "../../ -G \"Visual Studio 12\" -T \"v120\"",
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
    .IsDependentOn("Get-Git-Revision")
    .Does(() =>
    {
        MSBuild("./build/hadouken.sln", s =>
        {
            s.Configuration = Configuration;
        });
    });

Task("Output")
    .IsDependentOn("Compile")
    .Does(() =>
    {
        // Copy JS files
        CopyFiles(GetFiles("../js/*.js"),         BinariesDirectory + "/js");
        CopyFiles(GetFiles("../js/plugins/*.js"), BinariesDirectory + "/js/plugins");
        CopyFiles(GetFiles("../js/rpc/*.js"),     BinariesDirectory + "/js/rpc");

        // Copy relevant dist files
        var distFiles = new []
        {
            "../dist/win32/hadouken.json.template"
        };

        CopyFiles(distFiles, BinariesDirectory);
    });

Task("Create-Zip-Package")
    .IsDependentOn("Output")
    .Does(() =>
    {
        IEnumerable<FilePath> files = GetFiles(BinariesDirectory + "/*.dll");
        files = files.Union(GetFiles(BinariesDirectory + "/*.exe"));
        files = files.Union(GetFiles(BinariesDirectory + "/*.template"));
        files = files.Union(GetFiles(BinariesDirectory + "/js/*.js"));
        files = files.Union(GetFiles(BinariesDirectory + "/js/plugins/*.js"));
        files = files.Union(GetFiles(BinariesDirectory + "/js/rpc/*.js"));

        Zip(BinariesDirectory,
            OutputDirectory + "/hadouken-" + Version + VersionSuffix + ".zip",
            files);
    });

Task("Create-Pdb-Package")
    .IsDependentOn("Output")
    .Does(() =>
    {
        var pdbFiles = GetFiles(BinariesDirectory + "/*.pdb");

        Zip(BinariesDirectory,
            OutputDirectory + "/hadouken-" + Version + VersionSuffix + ".symbols.zip",
            pdbFiles);
    });

Task("Create-Msi-Package")
    .IsDependentOn("Output")
    .Does(() =>
    {
        var commitish = (GitCommitish.Length > 7) ? GitCommitish.Substring(0, 7) : GitCommitish;

        WiXCandle("./installer/**/*.wxs", new CandleSettings
        {
            Defines = new Dictionary<string, string>
            {
                { "BinDir",             BinariesDirectory },
                { "BuildConfiguration", Configuration },
                { "BuildVersion",       Version },
                { "GitCommitish",       commitish }
            },
            Extensions = new []
            {
                "WixFirewallExtension",
                "./tools/msiext-1.4/WixExtensions/WixSystemToolsExtension.dll"
            },
            OutputDirectory = "./build/wixobj",
            ToolPath = "./libs/WiX.Toolset/tools/wix/candle.exe"
        });

        WiXLight("./build/wixobj/*.wixobj", new LightSettings
        {
            Extensions = new []
            {
                "WixUtilExtension",
                "WixFirewallExtension",
                "./tools/msiext-1.4/WixExtensions/WixSystemToolsExtension.dll"
            },
            OutputFile = OutputDirectory + "/hadouken-" + Version + VersionSuffix + ".msi",
            ToolPath = "./libs/WiX.Toolset/tools/wix/light.exe",
            RawArguments = "-spdb -loc \"installer/lang/en-us.wxl\""
        });
    });

Task("Create-Chocolatey-Package")
    .IsDependentOn("Create-Msi-Package")
    .Does(() =>
    {
        var transformed = TransformTextFile("./chocolatey/tools/chocolateyInstall.ps1.template", "%{", "}")
                              .WithToken("File", "hadouken-" + Version + VersionSuffix + ".msi")
                              .WithToken("Version", Version)
                              .ToString();

        System.IO.File.WriteAllText("./chocolatey/tools/chocolateyInstall.ps1", transformed);

        var packSettings = new NuGetPackSettings
        {
            BasePath = "./chocolatey",
            OutputDirectory = OutputDirectory,
            Version = Version + VersionSuffix
        };

        NuGetPack("./chocolatey/hadouken.nuspec", packSettings);
    });

Task("Generate-Checksum-File")
    .IsDependentOn("Create-Zip-Package")
    .IsDependentOn("Create-Pdb-Package")
    .IsDependentOn("Create-Msi-Package")
    .Does(() =>
    {
        var checksums = new Dictionary<string, string>();

        foreach (var file in GetFiles(OutputDirectory + "/*.*"))
        {
            var hash = CalculateFileHash(file, HashAlgorithm.MD5);
            checksums.Add(file.GetFilename().ToString(), hash.ToHex());
        }

        System.IO.File.WriteAllText(OutputDirectory + "/checksums.json", ToJson(checksums));
    });

Task("Publish-Release")
    .Does(() =>
    {
        var data = ToJson(new
        {
            tag_name = "v" + Version + VersionSuffix,
            target_commitish = GitCommitish,
            name = "Hadouken " + Version + VersionSuffix,
            prerelease = !string.IsNullOrEmpty(VersionSuffix)
        });

        using(var client = new System.Net.WebClient())
        {
            client.Headers.Add("Accept", "application/vnd.github.v3+json");
            client.Headers.Add("Authorization", "token " + GitHubToken);
            client.Headers.Add("User-Agent", "Hadouken-Deployment-Agent");
            
            var response = client.UploadString("https://api.github.com/repos/" + GitHubRepository + "/releases", data);
            var responseObject = FromJson<IDictionary<string, object>>(response);
            var uploadUrlTemplate = responseObject["upload_url"].ToString();

            Information("Release created... Publishing files.");

            var toPublish = new []
            {
                OutputDirectory + "/checksums.json",
                OutputDirectory + "/hadouken-" + Version + VersionSuffix + ".msi",
                OutputDirectory + "/hadouken-" + Version + VersionSuffix + ".symbols.zip",
                OutputDirectory + "/hadouken-" + Version + VersionSuffix + ".zip"
            };

            foreach (var file in toPublish)
            {
                client.Headers["Content-Type"] = "application/octet-stream";

                var fileName = System.IO.Path.GetFileName(file);
                var uploadUrl = uploadUrlTemplate.Replace("{?name}", "?name=" + fileName);
                var uploadResponse = client.UploadData(uploadUrl, System.IO.File.ReadAllBytes(file));
                
                Information("File {0} published to GitHub.", fileName);
            }
        }

        // Publish nupkg to chocolatey
        var pushSettings = new NuGetPushSettings
        {
            ApiKey = ChocolateyApiKey,
            Source = "https://chocolatey.org/"
        };

        NuGetPush(OutputDirectory + "/hadouken." + Version + VersionSuffix + ".nupkg", pushSettings);
    });

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Prepare-Version-Suffix")
    .IsDependentOn("Compile")
    .IsDependentOn("Output")
    .IsDependentOn("Create-Zip-Package")
    .IsDependentOn("Create-Pdb-Package")
    .IsDependentOn("Create-Msi-Package")
    .IsDependentOn("Create-Chocolatey-Package")
    .IsDependentOn("Generate-Checksum-File");

Task("Publish")
    .IsDependentOn("Default")
    .IsDependentOn("Publish-Release");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(Target);
