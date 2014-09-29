using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.Data;
using Hadouken.Core.Data.Models;

namespace Hadouken.Core.Data
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly IDbConnection _connection;

        public UserRepository(IDbConnection connection)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            _connection = connection;
        }

        public IEnumerable<User> GetUsers()
        {
            var query = @"select u.Id, u.UserName, u.HashedPassword, u.Token from User u";
            return _connection.Query<User>(query);
        }

        public User GetByUserName(string userName)
        {
            var query = @"select u.Id, u.UserName, u.HashedPassword, u.Token from User u where u.UserName = @UserName";
            return _connection.Query<User>(query, new {UserName = userName}).FirstOrDefault();
        }

        public User GetByToken(string token)
        {
            var query = @"select u.Id, u.UserName, u.HashedPassword, u.Token from User u where u.Token = @Token";
            var user = _connection.Query<User>(query, new {Token = token}).SingleOrDefault();

            if (user == null)
            {
                // todo: check the generated tokens table
            }

            return user;
        }

        public void CreateUser(User user)
        {
            var query = @"insert into User (Id, UserName, HashedPassword, Token) values (@Id, @UserName, @HashedPassword, @Token);";
            _connection.Execute(query, user);
        }

        public void UpdatePassword(string userName, string hashedPassword)
        {
            var query = @"update User set HashedPassword = @HashedPassword where UserName = @UserName";
            _connection.Execute(query, new {UserName = userName, HashedPassword = hashedPassword});
        }

        public void UpdateUserToken(Guid userId, string token)
        {
            var query = @"update User set Token = @Token where Id = @Id";
            _connection.Execute(query, new {Id = userId.ToString(), Token = token});
        }
    }
}