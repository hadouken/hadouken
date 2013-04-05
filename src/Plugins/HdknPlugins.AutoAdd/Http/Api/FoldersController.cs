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
    }
}
