using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Http.Mvc
{
    public abstract class Controller
    {
        public IHttpContext Context { get; set; }

        protected TModel BindModel<TModel>()
        {
            return default(TModel);
        }
    }
}
