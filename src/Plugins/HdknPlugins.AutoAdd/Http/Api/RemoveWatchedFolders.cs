using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;
using Hadouken.Data;
using HdknPlugins.AutoAdd.Data.Models;

namespace HdknPlugins.AutoAdd.Http.Api
{
    [ApiAction("autoadd-remwatchedfolders")]
    public class RemoveWatchedFolders : ApiAction
    {
        private readonly IDataRepository _dataRepository;

        public RemoveWatchedFolders(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public override ActionResult Execute()
        {
            var ids = BindModel<int[]>();
            var idsRemoved = new List<int>();

            foreach(var id in ids)
            {
                var folder = _dataRepository.Single<WatchedFolder>(id);

                if(folder != null)
                {
                    _dataRepository.Delete(folder);
                    idsRemoved.Add(id);
                }
            }

            return Json(new {removed = idsRemoved.ToArray()});
        }
    }
}
