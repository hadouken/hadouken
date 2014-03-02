using System;
using System.Collections.Generic;
using Hadouken.Configuration;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;

namespace Hadouken.Http.Management
{
    public class HadoukenUserMapper : IUserMapper
    {
        private readonly IConfiguration _configuration;

        public HadoukenUserMapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            return new HadoukenIdentity {UserName = _configuration.Http.Authentication.UserName};
        }
    }

    public class HadoukenIdentity : IUserIdentity
    {
        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; set; }
    }
}
