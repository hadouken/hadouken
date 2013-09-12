using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.IO
{
    public interface IFileSystem
    {
        void CreateDirectory(string path);

        string[] GetDirectoryEntries(string path);

        bool IsDirectory(string path);
    }
}
