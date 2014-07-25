using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Hadouken.Common.Data;
using Hadouken.Core.Http.Config;

namespace Hadouken.Core.Http.Security
{
    public class UserManager : IUserManager
    {
        private readonly IKeyValueStore _keyValueStore;

        public UserManager(IKeyValueStore keyValueStore)
        {
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            _keyValueStore = keyValueStore;
        }

        public IEnumerable<User> GetUsers()
        {
            var users = _keyValueStore.Get<IList<User>>("http.users") ?? new List<User>();;
            return users;
        }

        public User GetUser(string username, string password)
        {
            var users = _keyValueStore.Get<IList<User>>("http.users") ?? new List<User>();
            var passwordHash = Hash(password);

            return users.SingleOrDefault(u => u.UserName == username
                                              && u.HashedPassword == passwordHash);
        }

        public void CreateUser(string username, string password)
        {
            var users = _keyValueStore.Get<IList<User>>("http.users") ?? new List<User>();

            if (users.SingleOrDefault(u => u.UserName == username) != null)
            {
                throw new Exception("User already exist.");
            }

            users.Add(new User
            {
                UserName = username,
                HashedPassword = Hash(password)
            });

            _keyValueStore.Set("http.users", users);
        }

        private static string Hash(string input)
        {
            var sb = new StringBuilder();

            using (var sha = new SHA512Managed())
            {
                var data = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(data);

                foreach (var b in hash)
                {
                    sb.AppendFormat("{0:x2}", b);
                }
            }

            return sb.ToString();
        }
    }
}