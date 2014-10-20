using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hadouken.Common.Text.BEncoding
{
    public class BDecoder
    {
        public BEncodedValue Decode(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                return Decode(reader);
            }
        }

        private BEncodedValue Decode(BinaryReader reader)
        {
            var c = reader.PeekChar();

            if (c == -1)
            {
                throw new InvalidDataException("Unexpected end of stream.");
            }

            switch (c)
            {
                case 'd':
                    return DecodeDictionary(reader);

                case 'i':
                    return DecodeNumber(reader);

                case 'l':
                    return DecodeList(reader);

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return DecodeString(reader);
            }

            return null;
        }

        private BEncodedDictionary DecodeDictionary(BinaryReader reader)
        {
            Expect(reader, 'd');

            var map = new Dictionary<BEncodedString, BEncodedValue>();

            while (true)
            {
                var key = Decode(reader);
                if (key == null) break;

                var value = Decode(reader);
                if (value == null) break;

                map.Add((BEncodedString) key, value);
            }

            Expect(reader, 'e');

            return new BEncodedDictionary(map);
        }

        private BEncodedList DecodeList(BinaryReader reader)
        {
            Expect(reader, 'l');

            var items = new List<BEncodedValue>();
            BEncodedValue value;

            while ((value = Decode(reader)) != null)
            {
                items.Add(value);
            }

            Expect(reader, 'e');

            return new BEncodedList(items);
        }

        private static BEncodedNumber DecodeNumber(BinaryReader reader)
        {
            Expect(reader, 'i');

            var builder = new StringBuilder();

            while (reader.PeekChar() != 'e')
            {
                builder.Append(reader.ReadChar());
            }

            Expect(reader, 'e');

            var num = long.Parse(builder.ToString());
            return new BEncodedNumber(num);
        }

        private static BEncodedString DecodeString(BinaryReader reader)
        {
            var builder = new StringBuilder();

            while (reader.PeekChar() != ':')
            {
                builder.Append(reader.ReadChar());
            }

            Expect(reader, ':');

            var length = int.Parse(builder.ToString());
            var data = reader.ReadBytes(length);

            return new BEncodedString(data);
        }

        private static void Expect(BinaryReader reader, char c)
        {
            var found = reader.ReadChar();

            if (found != c)
            {
                throw new InvalidDataException(string.Format("Expected {0}. Found {1}.", c, found));
            }
        }
    }
}
