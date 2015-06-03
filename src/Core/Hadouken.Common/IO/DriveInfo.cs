using System;
using System.IO;

namespace Hadouken.Common.IO {
    public class DriveInfo : IDriveInfo {
        private readonly System.IO.DriveInfo _driveInfo;

        public DriveInfo(System.IO.DriveInfo driveInfo) {
            if (driveInfo == null) {
                throw new ArgumentNullException("driveInfo");
            }
            this._driveInfo = driveInfo;
        }

        public long AvailableFreeSpace {
            get { return this._driveInfo.IsReady ? this._driveInfo.AvailableFreeSpace : -1; }
        }

        public DriveType DriveType {
            get { return this._driveInfo.DriveType; }
        }

        public bool IsReady {
            get { return this._driveInfo.IsReady; }
        }

        public string Name {
            get { return this._driveInfo.Name; }
        }

        public IDirectory RootDirectory {
            get { return new Directory(this._driveInfo.RootDirectory.FullName); }
        }

        public long TotalFreeSpace {
            get { return this._driveInfo.IsReady ? this._driveInfo.TotalFreeSpace : -1; }
        }

        public long TotalSize {
            get { return this._driveInfo.IsReady ? this._driveInfo.TotalSize : -1; }
        }

        public string VolumeLabel {
            get { return this._driveInfo.VolumeLabel; }
        }
    }
}