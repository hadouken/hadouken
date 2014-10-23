using System;
using System.Collections.Generic;

namespace Hadouken.Tools.Posh
{
    public interface IRuntime
    {
        string AccessToken { get; }

        Uri Url { get; }

        ICollection<string> GetResolvedPaths(string path);

        void WriteObject(object obj, bool enumerateCollection);
    }
}
