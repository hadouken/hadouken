namespace Hadouken.Framework.Security
{
    public abstract class HashProvider : IHashProvider
    {
        public abstract string ComputeHash(string input);

        public abstract byte[] ComputeHash(byte[] input);

        public static IHashProvider Default()
        {
            return new Sha256HashProvider();
        }
    }
}