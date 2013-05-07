using System;

// boldly borrowed from
// http://blog.peterlesliemorris.com/archive/2011/04/02/a-webserver-for-monotouch-that-also-parses-posted-form-data.aspx

namespace Hadouken.Http.HttpServer
{
    public class MultiPartStreamValue
    {
        public string Name { get; private set; }

        public MultiPartStreamValue(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Name");

            Name = name;
        }

    }

    public class MultiPartStreamSimpleValue : MultiPartStreamValue
    {
        public string Value { get; private set; }

        public MultiPartStreamSimpleValue(string name, string value)
            : base(name)
        {
            Value = value;
        }
    }

    public class MultiPartStreamFileValue : MultiPartStreamValue
    {
        public string FileName { get; private set; }
        public byte[] FileData { get; private set; }
        public string ContentType { get; private set; }

        public MultiPartStreamFileValue(string name, string fileName, byte[] fileData, string contentType)
            : base(name)
        {
            if (fileData == null)
                throw new ArgumentNullException("FileData");
            if (string.IsNullOrEmpty(fileName) && fileData.Length > 0)
                throw new ArgumentNullException("FileName");
            if (string.IsNullOrEmpty(contentType))
                throw new ArgumentNullException("Content-Type");

            FileName = fileName;
            FileData = fileData;
            ContentType = contentType;
        }
    }
}
