using Hadouken.Common.Data;
using Hadouken.Common.Http.Mvc;
using HdknPlugins.AutoAdd.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HdknPlugins.AutoAdd.Http.Api
{
    public class FoldersController : Controller
    {
        private readonly IDataRepository _dataRepository;

        public FoldersController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        [HttpGet("api/folders")]
        public ActionResult List()
        {
            return new JsonResult(_dataRepository.List<Folder>());
        }

        [HttpPost("api/folders")]
        public ActionResult Create()
        {
            var folder = BindModel<Folder>();

            if (folder != null)
            {
                folder.Id = 0;
                _dataRepository.Save(folder);
            }

            return new JsonResult(folder);
        }

        [HttpPut("api/folders/{id}")]
        public ActionResult Edit(int id)
        {
            var folder = BindModel<Folder>();
            var oldFolder = _dataRepository.Single<Folder>(id);

            if (oldFolder != null)
            {
                folder.Id = id;
                _dataRepository.Update(folder);
            }

            return new JsonResult(folder);
        }

        [HttpDelete("api/folders/{id}")]
        public ActionResult Delete(int id)
        {
            var folder = _dataRepository.Single<Folder>(id);

            if (folder != null)
            {
                _dataRepository.Delete(folder);

                return new JsonResult(true);
            }

            return new JsonResult(false);
        }
    }
}
