using Hadouken.Common;
using Hadouken.Configuration;

namespace Hadouken.Impl
{
    [Component]
    public class HostEnvironment : IEnvironment
    {
        public string ConnectionString
        {
            get { return HdknConfig.ConnectionString; }
        }
    }
}
