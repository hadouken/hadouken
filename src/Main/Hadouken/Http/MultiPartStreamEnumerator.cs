using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

using Hadouken.Extensions;

// boldly borrowed from
// http://blog.peterlesliemorris.com/archive/2011/04/02/a-webserver-for-monotouch-that-also-parses-posted-form-data.aspx

namespace Hadouken.Http
{
    public class MultiPartStreamEnumerator : IEnumerator<MultiPartStreamValue>
    {
        readonly Stream SourceStream;
        readonly Encoding Encoding;
        readonly byte[] BoundaryIndicator;
        readonly byte[] FinalBoundaryIndicator;

        bool CanMoveNext = false;
        MultiPartStreamValue LastValue;

        public MultiPartStreamEnumerator(Stream sourceStream, Encoding encoding, string boundaryIndicator)
        {
            if (sourceStream == null)
                throw new ArgumentNullException("SourceStream");
            if (encoding == null)
                throw new ArgumentNullException("Encoding");
            if (string.IsNullOrEmpty(boundaryIndicator))
                throw new ArgumentNullException("BoundaryIndicator");

            SourceStream = sourceStream;
            Encoding = encoding;
            BoundaryIndicator = Encoding.GetBytes("--" + boundaryIndicator + "\r\n");
            FinalBoundaryIndicator = Encoding.GetBytes("--" + boundaryIndicator + "--\r\n");

            //Initialise
            byte[] firstLine = ReadLineAsBytes();
            if (IsBoundary(firstLine))
                CanMoveNext = true;
            else if (IsFinalBoundary(firstLine))
                CanMoveNext = false;
            else
                throw new EndOfStreamException("Malformed post");
        }

        public MultiPartStreamValue Current
        {
            get { return LastValue; }
        }

        public void Dispose()
        {
        }

        bool CompareBytes(byte[] source, byte[] comparison, int count)
        {
            if (source.Length < count)
                throw new ArgumentException("Source.Length is too short");
            if (comparison.Length < count)
                return false;

            for (int index = 0; index < count; index++)
                if (source[index] != comparison[index])
                    return false;
            return true;
        }

        bool IsBoundary(byte[] bytes)
        {
            return CompareBytes(BoundaryIndicator, bytes, BoundaryIndicator.Length);
        }

        bool IsFinalBoundary(byte[] bytes)
        {
            return CompareBytes(FinalBoundaryIndicator, bytes, FinalBoundaryIndicator.Length);
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            LastValue = null;
            if (!CanMoveNext)
                return false;
            LastValue = ParsePostedValue();
            return true;
        }

        public void Reset()
        {
            throw new InvalidOperationException();
        }

        byte[] ReadLineAsBytes()
        {
            var resultStream = new MemoryStream();
            while (true)
            {
                int data = SourceStream.ReadByte();
                resultStream.WriteByte((byte)data);
                if (data == 10)
                    break;
            }
            resultStream.Position = 0;
            byte[] dataBytes = new byte[resultStream.Length];
            resultStream.Read(dataBytes, 0, dataBytes.Length);
            return dataBytes;
        }

        string ReadLineWithoutCRLF()
        {
            string result = Encoding.GetString(ReadLineAsBytes());
            if (result.EndsWith("\r\n"))
                result = result.Substring(0, result.Length - 2);
            return result;
        }

        MultiPartStreamValue ParsePostedValue()
        {
            var headers = new NameValueCollection();
            while (true)
            {
                string line = ReadLineWithoutCRLF();
                if (line == "")
                    break;
                var nameAndValue = line.ToNameAndValue(':');
                headers.Add(nameAndValue.Key, nameAndValue.Value);
            }
            string contentDispositionLine = headers["content-disposition"];
            if (contentDispositionLine == null)
                throw new InvalidOperationException("Content-Disposition expected");
            contentDispositionLine = RemoveFirstValue(contentDispositionLine);
            NameValueCollection contentDispositionValues = contentDispositionLine.ToNameValueCollection();

            string name = contentDispositionValues["Name"];
            if (name != null)
                name = name.UnQuote();
            if (string.IsNullOrEmpty(name))
                throw new InvalidOperationException("Name missing in Content-Disposition header");

            byte[] binaryData = ReadUntilNextBoundary();

            MultiPartStreamValue result;
            string fileName = contentDispositionValues["FileName"];
            if (fileName != null)
                result = new MultiPartStreamFileValue(name, fileName.UnQuote(), binaryData, headers["Content-Type"]);
            else
                result = new MultiPartStreamSimpleValue(name, Encoding.GetString(binaryData));
            return result;
        }

        string RemoveFirstValue(string source, char separator = ';')
        {
            return string.Join(separator.ToString(), source.Split(separator).Skip(1).ToArray());
        }

        byte[] ReadUntilNextBoundary()
        {
            var resultStream = new MemoryStream();
            do
            {
                byte[] currentChunk = ReadLineAsBytes();
                if (IsBoundary(currentChunk))
                {
                    CanMoveNext = true;
                    break;
                }
                if (IsFinalBoundary(currentChunk))
                {
                    CanMoveNext = false;
                    break;
                }
                resultStream.Write(currentChunk, 0, currentChunk.Length);
            } while (true);

            //Remove 2 chars for CRLF
            byte[] result = new byte[resultStream.Length - 2];
            resultStream.Position = 0;
            resultStream.Read(result, 0, result.Length);
            return result;
        }


    }
}
