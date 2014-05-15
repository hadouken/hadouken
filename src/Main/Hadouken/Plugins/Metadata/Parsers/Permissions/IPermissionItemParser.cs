using System.Security;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata.Parsers.Permissions
{
    public interface IPermissionItemParser
    {
        IPermission Parse(JToken token);

        IPermission GetUnrestricted();
    }
}
