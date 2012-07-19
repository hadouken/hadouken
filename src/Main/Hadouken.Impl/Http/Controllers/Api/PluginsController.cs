using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.Http;
using Hadouken.Plugins;
using Hadouken.IO;
using System.IO;
using Hadouken.Configuration;
using Hadouken.Reflection;
using Hadouken.Data.Models;
using Hadouken.Data;

namespace Hadouken.Impl.Http.Controllers.Api
{
    public class PluginsController : Controller
    {
        private IDataRepository _repo;
        private IPluginEngine _engine;
        private IPluginLoader[] _loaders;
        private IFileSystem _fs;

        public PluginsController(IDataRepository repo, IPluginEngine engine, IPluginLoader[] loaders, IFileSystem fs)
        {
            _repo = repo;
            _engine = engine;
            _loaders = loaders;
            _fs = fs;
        }

        [HttpGet]
        [Route("/api/plugins")]
        public ActionResult List()
        {
            return Json((from man in _engine.Managers.Values
                         select new { Name = man.Name, Version = man.Version }));
        }

        [HttpPost]
        [Route("/api/plugins")]
        public ActionResult Create()
        {
            var info = BindModel<PluginInfo>();
            info.Id = 0;

            _repo.Save(info);

            return Json(true);
        }

        [HttpPost]
        [Route("/api/plugins/upload")]
        public ActionResult UploadPlugin()
        {
            List<PluginInfo> newPlugins = new List<PluginInfo>();

            foreach (var file in Context.Request.Files)
            {
                // save file to disk
                var ms = new MemoryStream();
                file.InputStream.CopyTo(ms);

                string path = Path.Combine(HdknConfig.GetPath("Paths.Plugins"), Path.GetFileName(file.FileName));

                _fs.WriteAllBytes(path, ms.ToArray());

                // use a loader to determine if it is a plugin file, if not - delete
                IPluginLoader loader = (from l in _loaders
                                        where l.CanLoad(path)
                                        select l).First();

                Type pluginType = loader.Load(path).First();

                if (pluginType.HasAttribute<PluginAttribute>())
                {
                    PluginAttribute attr = pluginType.GetAttribute<PluginAttribute>();
                    
                    PluginInfo info = new PluginInfo();
                    info.Name = attr.Name;
                    info.Version = attr.Version;
                    info.Path = path;
                    info.State = PluginState.Uninstalled;

                    _repo.Save(info);

                    _engine.Refresh();

                    newPlugins.Add(info);
                }
            }

            return Json(from i in newPlugins select new { Name = i.Name, Version = i.Version });
        }
    }
}
