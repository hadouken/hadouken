using System.IO;
using System.Text;

namespace Hadouken.Common.Text.BEncoding
{
    public static class BDecoderExtensions
    {
        public static BEncodedValue Decode(this BDecoder decoder, string value)
        {
            var data = Encoding.UTF8.GetBytes(value);

            using (var stream = new MemoryStream(data))
            {
                return decoder.Decode(stream);
            }
        }
    }
}
