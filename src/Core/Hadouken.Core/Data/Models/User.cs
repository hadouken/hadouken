using System;

namespace Hadouken.Core.Data.Models {
    public sealed class User {
        public User() {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string HashedPassword { get; set; }
        public string Token { get; set; }
    }
}