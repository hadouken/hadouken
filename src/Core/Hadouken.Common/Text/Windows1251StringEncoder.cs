using System.Text;

namespace Hadouken.Common.Text
{
    public class Windows1251StringEncoder : IStringEncoder
    {
        public string Encode(string localString)
        {
            Encoding utf = Encoding.UTF8;
            byte[] winBytes = Encoding.GetEncoding("windows-1251").GetBytes(localString);
            return utf.GetString(winBytes);
        }
    }
}