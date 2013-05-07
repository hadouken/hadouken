using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;
using Hadouken.Data;
using HdknPlugins.AutoAdd.Data.Models;

namespace HdknPlugins.AutoAdd.Http.Api
{
    [ApiAction("autoadd-setwatchedfolders")]
    public class SetWatchedFolders : ApiAction
    {
        private readonly IDataRepository _dataRepository;

        public SetWatchedFolders(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public override ActionResult Execute()
        {
            var folders = BindModel<WatchedFolder[]>();

            foreach(var folder in folders)
            {
                _dataRepository.SaveOrUpdate(folder);
            }

            return Json(folders);
        }
    }
}
