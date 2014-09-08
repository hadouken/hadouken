using System;
using System.Linq;
using Hadouken.Core.Data;
using Hadouken.Core.Data.Models;
using System.Security.Cryptography;

namespace Hadouken.Core.Security
{
    public sealed class UserManager : IUserManager
    {
        private static readonly int DefaultHashIterations = 10000;
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
            var salt = new byte[32];            
            var rnd = new Random(DateTime.Now.Millisecond);
            rnd.NextBytes(salt);

            var hash = HashPassword(password, salt, DefaultHashIterations);

            var hashedPassword = string.Format("{0}|{1}|{2}", 
                DefaultHashIterations,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash));

            var u = new User { UserName = userName, HashedPassword = hashedPassword };
            _userRepository.CreateUser(u);
        }

        public IUser GetUser(string userName, string password)
        {
            var user = _userRepository.GetByUserName(userName);
            if(user == null) return null;

            var hashedParts = user.HashedPassword.Split('|');
            var iterations = int.Parse(hashedParts[0]);
            var salt = Convert.FromBase64String(hashedParts[1]);
            var storedHash = Convert.FromBase64String(hashedParts[2]);

            var hash = HashPassword(password, salt, iterations);

            if (!storedHash.SequenceEqual(hash))
            {
                return null;
            }

            return new HadoukenUser(Guid.Parse(user.Id), user.UserName);
        }

        private byte[] HashPassword(string password, byte[] salt, int iterationCount)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterationCount);
            return pbkdf2.GetBytes(64);
        }
    }
}