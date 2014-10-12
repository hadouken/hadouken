using System;

namespace Hadouken.Core.Security
{
    public interface IUser
    {
        Guid Id { get; }

        string UserName { get; }

        string[] Claims { get; }

        string Token { get; }
    }
}
