using System;
using System.Collections.Generic;
using Hadouken.Core.Data.Models;

namespace Hadouken.Core.Data
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();

        User GetByUserName(string userName);

        User GetByToken(string token);

        void CreateUser(User user);

        void UpdatePassword(string userName, string hashedPassword);
        
        void UpdateUserToken(Guid userId, string token);
    }
}
