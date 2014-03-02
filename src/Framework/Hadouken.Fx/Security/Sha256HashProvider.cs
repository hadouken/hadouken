using System.Security.Cryptography;
using System.Text;

namespace Hadouken.Fx.Security
{
    public class Sha256HashProvider : HashProvider
    {
        public override string ComputeHash(string input)
        {
            var data = Encoding.UTF8.GetBytes(input);
            var hash = ComputeHash(data);
            var sb = new StringBuilder();

            foreach (var b in hash)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            return sb.ToString();
        }

        public override byte[] ComputeHash(byte[] input)
        {
            var sha256 = new SHA256Managed();
            return sha256.ComputeHash(input);
        }
    }
}