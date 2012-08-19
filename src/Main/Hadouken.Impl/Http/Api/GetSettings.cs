using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Hadouken.Configuration;
using Hadouken.Http;
using Hadouken.Reflection;
using Hadouken.Data.Models;

namespace Hadouken.Impl.Http.Api
{
    [ApiAction("getsettings")]
    public class GetSettings : ApiAction
    {
        private IKeyValueStore _data;

        public GetSettings(IKeyValueStore data)
        {
            _data = data;
        }

        public override ActionResult Execute(IHttpContext context)
        {
            return Json(
                new
                {
                    version = typeof(Kernel).Assembly.GetAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion,
                    settings = (from setting in _data.Get(s => true)
                                select new object[]
                                {
                                    setting.Key,
                                    GetSettingType(setting.Value.GetType()),
                                    setting.Value,
                                    new {
                                        access = -1
                                    }
                                })
                }
            );
        }

        private int GetSettingType(Type clrType)
        {
            switch (clrType.FullName)
            {
                case "System.Int32":
                    return 0;

                case "System.Boolean":
                    return 1;

                case "System.String":
                    return 2;

                default: // unknown (to webui)
                    throw new NotImplementedException(String.Format("Unknown Web UI type {0}", clrType));
            }
        }
    }
}
