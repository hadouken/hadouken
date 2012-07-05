using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Hadouken.Extensions;

// boldly borrowed from
// http://blog.peterlesliemorris.com/archive/2011/04/02/a-webserver-for-monotouch-that-also-parses-posted-form-data.aspx

namespace Hadouken.Http
{
    public class MultiPartStreamReader : IEnumerable<MultiPartStreamValue>
    {
        readonly Encoding Encoding;
        readonly Stream SourceStream;
        readonly string BoundaryIndicator;

        public MultiPartStreamReader(Stream sourceStream, Encoding encoding, string contentType)
        {
            if (sourceStream == null)
                throw new ArgumentNullException("SourceStream");
            if (encoding == null)
                throw new ArgumentNullException("Encoding");
            if (string.IsNullOrEmpty(contentType))
                throw new ArgumentNullException("ContentType");

            SourceStream = sourceStream;
            Encoding = encoding;
            BoundaryIndicator = GetBoundaryIndicator(contentType);
        }

        string GetBoundaryIndicator(string contentType)
        {
            if (!contentType.ToLowerInvariant().StartsWith("multipart/form-data"))
                throw new InvalidOperationException("Must be multipart/form-data");
            string[] values = contentType.Split(';').Skip(1).ToArray();
            var namesAndValues = string.Join(";", values);
            return namesAndValues.ToNameValueCollection()["Boundary"];
        }


        public IEnumerator<MultiPartStreamValue> GetEnumerator()
        {
            return new MultiPartStreamEnumerator(SourceStream, Encoding, BoundaryIndicator);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
