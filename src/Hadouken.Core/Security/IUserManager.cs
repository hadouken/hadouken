namespace Hadouken.Core.Security
{
    public interface IUserManager
    {
        bool HasUsers();

        void CreateUser(string userName, string password);

        IUser GetUser(string userName, string password);

        void ChangePassword(string userName, string newPassword);
    }
}
