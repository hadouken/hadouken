namespace Hadouken.Security
{
    public interface IHashProvider
    {
        string ComputeHash(string input);

        byte[] ComputeHash(byte[] input);
    }
}
