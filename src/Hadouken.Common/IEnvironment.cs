using Hadouken.Common.IO;

namespace Hadouken.Common
{
    public interface IEnvironment
    {
        /// <summary>
        /// Gets whether or not the current operative system is 64 bit.
        /// </summary>
        /// <returns>Whether or not the current operative system is 64 bit.</returns>
        bool Is64BitOperativeSystem();

        /// <summary>
        /// Determines whether the current machine is running Unix.
        /// </summary>
        /// <returns>Whether or not the current machine is running Unix.</returns>
        bool IsUnix();

        /// <summary>
        /// Gets whether or not the current session (process) is user interactive (eg. has a console attached).
        /// </summary>
        /// <returns><c>true</c> if the session is user interactive, otherwise <c>false</c>.</returns>
        bool IsUserInteractive();

        /// <summary>
        /// Gets a special path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A <see cref="DirectoryPath"/> to the special path.</returns>
        DirectoryPath GetSpecialPath(SpecialPath path);

        /// <summary>
        /// Gets the application root path.
        /// </summary>
        /// <returns>The application root path.</returns>
        DirectoryPath GetApplicationRoot();

        /// <summary>
        /// Gets the application data path.
        /// </summary>
        /// <returns>The application data path.</returns>
        DirectoryPath GetApplicationDataPath();

        Path GetWebApplicationPath();

        string GetAppSetting(string key);

        string GetConnectionString(string name);

        /// <summary>
        /// Gets an environment variable.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <returns>The value of the environment variable.</returns>
        string GetEnvironmentVariable(string variable);

        /// <summary>
        /// Creates the paths needed for Hadouken to run (ie. the data path mostly)
        /// </summary>
        void Create();
    }
}
