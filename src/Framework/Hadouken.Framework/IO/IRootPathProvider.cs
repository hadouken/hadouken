using System;

namespace Hadouken.Framework.IO
{
    public interface IRootPathProvider
    {
        string GetRootPath();
    }

    public class RootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}