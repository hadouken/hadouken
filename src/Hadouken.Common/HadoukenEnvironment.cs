using System;
using System.Configuration;
using System.Reflection;
using System.Text.RegularExpressions;
using Hadouken.Common.IO;

namespace Hadouken.Common
{
    public class HadoukenEnvironment : IEnvironment
    {
        public bool Is64BitOperativeSystem()
        {
            return Machine.Is64BitOperativeSystem();
        }

        public bool IsUnix()
        {
            return Machine.IsUnix();
        }

        public bool IsUserInteractive()
        {
            return Environment.UserInteractive;
        }

        public DirectoryPath GetSpecialPath(SpecialPath path)
        {
            if (path == SpecialPath.ProgramFilesX86)
            {
                return new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
            }
            if (path == SpecialPath.Windows)
            {
                return new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.Windows));
            }
            const string format = "The special path '{0}' is not supported.";
            throw new NotSupportedException(string.Format(format, path));
        }

        public DirectoryPath GetApplicationRoot()
        {
            var path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return new DirectoryPath(path);
        }

        public DirectoryPath GetApplicationDataPath()
        {
            var path = GetAppSetting("Path:Data");
            return new DirectoryPath(path);
        }

        public Path GetWebApplicationPath()
        {
            var path = GetAppSetting("Path:Web");

            if (path.EndsWith(".zip")) return new FilePath(path);
            return new DirectoryPath(path);
        }

        public string GetEnvironmentVariable(string variable)
        {
            return Environment.GetEnvironmentVariable(variable);
        }

        public string GetAppSetting(string key)
        {
            var replaced = ReplaceAppSettingTokens(ConfigurationManager.AppSettings[key]);
            return Environment.ExpandEnvironmentVariables(replaced);
        }

        public string GetConnectionString(string name)
        {
            return ReplaceAppSettingTokens(ConfigurationManager.ConnectionStrings[name].ConnectionString);
        }

        private string ReplaceAppSettingTokens(string input)
        {
            const string pattern = @"(\${([a-zA-Z\.\:]+)})";
            var matches = Regex.Matches(input, pattern);

            foreach (Match match in matches)
            {
                var token = match.Groups[1].Value;
                var value = match.Groups[2].Value;

                input = input.Replace(token, GetAppSetting(value));
            }

            return input;
        }
    }
}