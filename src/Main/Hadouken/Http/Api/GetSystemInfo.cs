using Hadouken.Reflection;

namespace Hadouken.Http.Api
{
    [ApiAction("getsysteminfo")]
    public class GetSystemInfo : ApiAction
    {
        public override ActionResult Execute()
        {
            return Json(new
                            {
                                version = typeof (Kernel).Assembly.GetName().Version.ToString(),
                                buildDate = typeof(Kernel).Assembly.GetAttribute<BuildDateAttribute>().BuildDate.ToUnixTime()
                            });
        }
    }
}
