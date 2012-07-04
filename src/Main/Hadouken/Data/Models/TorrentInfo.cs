using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.BitTorrent;

namespace Hadouken.Data.Models
{
    public class TorrentInfo : IModel
    {
        public virtual int Id { get; set; }
        public virtual string InfoHash { get; set; }
        public virtual byte[] Data { get; set; }
        public virtual byte[] FastResumeData { get; set; }
        public virtual long DownloadedBytes { get; set; }
        public virtual long UploadedBytes { get; set; }
        public virtual TorrentState State { get; set; }
        public virtual string SavePath { get; set; }
        public virtual string Label { get; set; }
    }
}
