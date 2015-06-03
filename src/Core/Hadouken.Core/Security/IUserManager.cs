using System;

namespace Hadouken.Core.Security {
    public interface IUserManager {
        bool HasUsers();
        void CreateUser(string userName, string password);
        IUser GetUser(string userName, string password);
        IUser GetUserByToken(string token);
        void ChangePassword(string userName, string newPassword);
        string GenerateToken(Guid userId);
    }
}