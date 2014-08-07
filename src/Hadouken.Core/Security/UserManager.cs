using System;
using System.Linq;
using Hadouken.Core.Data;
using Hadouken.Core.Data.Models;

namespace Hadouken.Core.Security
{
    public sealed class UserManager : IUserManager
    {
        private readonly IUserRepository _userRepository;

        public UserManager(IUserRepository userRepository)
        {
            if (userRepository == null) throw new ArgumentNullException("userRepository");
            _userRepository = userRepository;
        }

        public bool HasUsers()
        {
            return _userRepository.GetUsers().Any();
        }

        public void CreateUser(string userName, string password)
        {
            var u = new User {UserName = userName, HashedPassword = password};
            _userRepository.CreateUser(u);
        }

        public IUser GetUser(string userName, string password)
        {
            throw new System.NotImplementedException();
        }
    }
}