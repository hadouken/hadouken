using System.IO;

namespace Hadouken.Common.IO
{
    public interface IDriveInfo
    {
        long AvailableFreeSpace { get; }

        DriveType DriveType { get; }

        bool IsReady { get; }

        string Name { get; }

        IDirectory RootDirectory { get; }

        long TotalFreeSpace { get; }

        long TotalSize { get; }

        string VolumeLabel { get; }
    }
}
