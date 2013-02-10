using System;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
using Hadouken.Reflection;
using Hadouken.Data;
using Hadouken.Data.Models;

namespace Hadouken.Http.Api
{
    [ApiAction("getsettings")]
    public class GetSettings : ApiAction
    {
        private readonly JavaScriptSerializer _javaScriptSerializer = new JavaScriptSerializer();
        private readonly IDataRepository _dataRepository;

        public GetSettings(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public override ActionResult Execute()
        {
            return Json(
                new
                {
                    version = typeof(Kernel).Assembly.GetAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion,
                    settings = (from setting in _dataRepository.List<Setting>()
                                where setting.Key != "auth.password" // do not send hashed password to webui
                                select new object[]
                                {
                                    setting.Key,
                                    GetSettingType(setting.Type),
                                    _javaScriptSerializer.Deserialize(setting.Value, Type.GetType(setting.Type)),
                                    setting.Permissions,
                                    setting.Options
                                })
                }
            );
        }

        private static int GetSettingType(string clrType)
        {
            switch (clrType)
            {
                case "System.Int32":
                    return 0;

                case "System.Boolean":
                    return 1;

                case "System.String":
                    return 2;

                default: // unknown (to webui)
                    return -1;
            }
        }
    }
}
