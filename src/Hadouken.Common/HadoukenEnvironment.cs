using System;
using System.Configuration;
using System.Reflection;
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
            var path = ConfigurationManager.AppSettings["AppData"];
            path = Environment.ExpandEnvironmentVariables(path);

            return new DirectoryPath(path);
        }

        public Path GetWebApplicationPath()
        {
            var path = ConfigurationManager.AppSettings["WebPath"];

            if (path.EndsWith(".zip")) return new FilePath(path);
            return new DirectoryPath(path);
        }

        public string GetEnvironmentVariable(string variable)
        {
            return Environment.GetEnvironmentVariable(variable);
        }
    }
}