using System.Security;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata.Permissions
{
    public interface IPermissionParser
    {
        IPermission Parse(JToken token);

        IPermission GetUnrestricted();
    }
}
