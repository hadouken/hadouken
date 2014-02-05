namespace Hadouken.Framework.Security
{
    public class UserValidator : IUserValidator
    {
        private readonly string _userName;
        private readonly string _password;
        private readonly IHashProvider _hashProvider;

        public UserValidator(string userName, string password)
        {
            _userName = userName;
            _password = password;
            _hashProvider = HashProvider.GetDefault();
        }

        public bool IsValidUser(string userName, string password)
        {
            var passwordHash = _hashProvider.ComputeHash(password);
            return userName == _userName && _password == passwordHash;
        }
    }
}