using System;
using System.Linq;
namespace Hadouken.Core.Security
{
    public class HadoukenUser : IUser
    {
        public HadoukenUser(Guid id, string userName, params string[] claims)
        {
            Id = id;
            UserName = userName;
            Claims = claims ?? Enumerable.Empty<string>().ToArray();
        }

        public Guid Id { get; private set; }

        public string UserName { get; private set; }

        public string[] Claims { get; private set; }
    }
}
