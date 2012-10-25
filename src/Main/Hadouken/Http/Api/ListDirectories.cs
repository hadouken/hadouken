using System.Collections.Generic;
using System.Linq;
using Hadouken.Configuration;
using Hadouken.IO;

namespace Hadouken.Http.Api
{
    [ApiAction("listdirs")]
    public class ListDirectories : ApiAction
    {
        private IKeyValueStore _keyValueStore;
        private IFileSystem _fileSystem;

        public ListDirectories(IKeyValueStore keyValueStore, IFileSystem fileSystem)
        {
            _keyValueStore = keyValueStore;
            _fileSystem = fileSystem;
        }

        public override ActionResult Execute()
        {
            var dirs = (from dir in _keyValueStore.Get("paths.favorites", new List<string>())
                             select new
                                        {
                                            path = dir,
                                            available = "" + (_fileSystem.RemainingDiskSpace(dir) / 1024 / 1024)
                                        }).ToList();

            var defaultSavePath = _keyValueStore.Get<string>("paths.defaultSavePath");

            dirs.Insert(0, new { path = "Default download dir", available = "" + (_fileSystem.RemainingDiskSpace(defaultSavePath) / 1024 / 1024) });

            return Json(new Dictionary<string, object> { { "download-dirs", dirs } });
        }
    }
}
