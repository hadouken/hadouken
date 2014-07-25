using System;
using System.Collections.Generic;

namespace Hadouken.Core.Http.Config
{
    public sealed class User
    {
        private readonly IList<string> _claims;

        public User()
        {
            Id = Guid.NewGuid();
            _claims = new List<string>();
        }

        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string HashedPassword { get; set; }

        public IEnumerable<string> Claims
        {
            get { return new List<string>(_claims); }
        }

        public void AddClaim(string claim)
        {
            _claims.Add(claim);
        }

        public void RemoveClaim(string claim)
        {
            _claims.Remove(claim);
        }
    }
}
