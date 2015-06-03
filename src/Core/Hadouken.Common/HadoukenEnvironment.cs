using System;
using System.Configuration;
using System.Reflection;
using System.Text.RegularExpressions;
using Hadouken.Common.IO;
using Directory = System.IO.Directory;

namespace Hadouken.Common {
    public class HadoukenEnvironment : IEnvironment {
        public bool Is64BitOperativeSystem() {
            return Machine.Is64BitOperativeSystem();
        }

        public bool IsUnix() {
            return Machine.IsUnix();
        }

        public bool IsUserInteractive() {
            return Environment.UserInteractive;
        }

        public DirectoryPath GetSpecialPath(SpecialPath path) {
            switch (path) {
                case SpecialPath.ProgramFilesX86:
                    return new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
                case SpecialPath.Windows:
                    return new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.Windows));
            }
            const string format = "The special path '{0}' is not supported.";
            throw new NotSupportedException(string.Format(format, path));
        }

        public DirectoryPath GetApplicationRoot() {
            var path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return new DirectoryPath(path);
        }

        public DirectoryPath GetApplicationDataPath() {
            var path = this.GetAppSetting("Path:Data");
            return new DirectoryPath(path);
        }

        public Path GetWebApplicationPath() {
            var path = this.GetAppSetting("Path:Web");

            if (path.EndsWith(".zip")) {
                return new FilePath(path);
            }
            return new DirectoryPath(path);
        }

        public string GetEnvironmentVariable(string variable) {
            return Environment.GetEnvironmentVariable(variable);
        }

        public void Create() {
            var path = this.GetApplicationDataPath();

            if (!Directory.Exists(path.FullPath)) {
                Directory.CreateDirectory(path.FullPath);
            }
        }

        public string GetAppSetting(string key) {
            var replaced = this.ReplaceAppSettingTokens(ConfigurationManager.AppSettings[key]);
            return Environment.ExpandEnvironmentVariables(replaced);
        }

        public string GetConnectionString(string name) {
            return this.ReplaceAppSettingTokens(ConfigurationManager.ConnectionStrings[name].ConnectionString);
        }

        private string ReplaceAppSettingTokens(string input) {
            const string pattern = @"(\${([a-zA-Z\.\:]+)})";
            var matches = Regex.Matches(input, pattern);

            foreach (Match match in matches) {
                var token = match.Groups[1].Value;
                var value = match.Groups[2].Value;

                input = input.Replace(token, this.GetAppSetting(value));
            }

            return input;
        }
    }
}