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
            var query = @"select u.Id, u.UserName, u.HashedPassword from User u";
            return _connection.Query<User>(query);
        }

        public User GetByUserName(string userName)
        {
            var query = @"select u.Id, u.UserName, u.HashedPassword from User u where u.UserName = @UserName";
            return _connection.Query<User>(query, new {UserName = userName}).FirstOrDefault();
        }

        public void CreateUser(User user)
        {
            var query = @"insert into User (Id, UserName, HashedPassword) values (@Id, @UserName, @HashedPassword);";
            _connection.Execute(query, user);
        }
    }
}