using System;
using System.IO;

namespace Hadouken.Common.IO
{
    public class DriveInfo : IDriveInfo
    {
        private readonly System.IO.DriveInfo _driveInfo;

        public DriveInfo(System.IO.DriveInfo driveInfo)
        {
            if (driveInfo == null) throw new ArgumentNullException("driveInfo");
            _driveInfo = driveInfo;
        }

        public long AvailableFreeSpace
        {
            get { return _driveInfo.IsReady ? _driveInfo.AvailableFreeSpace : -1; }
        }

        public DriveType DriveType
        {
            get { return _driveInfo.DriveType; }
        }

        public bool IsReady
        {
            get { return _driveInfo.IsReady; }
        }

        public string Name
        {
            get { return _driveInfo.Name; }
        }

        public IDirectory RootDirectory
        {
            get { return new Directory(_driveInfo.RootDirectory.FullName); }
        }

        public long TotalFreeSpace
        {
            get { return _driveInfo.IsReady ? _driveInfo.TotalFreeSpace : -1; }
        }

        public long TotalSize
        {
            get { return _driveInfo.IsReady ? _driveInfo.TotalSize : -1; }
        }

        public string VolumeLabel
        {
            get { return _driveInfo.VolumeLabel; }
        }
    }
}