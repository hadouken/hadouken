using System;
using Hadouken.Configuration;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;

namespace Hadouken.Http.Management.Security
{
    public class UserMapper : IUserMapper
    {
        private readonly IConfiguration _configuration;

        public UserMapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            return new UserIdentity(_configuration.Http.Authentication.UserName);
        }
    }
}
