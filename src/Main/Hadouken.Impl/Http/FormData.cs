using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

using Hadouken.Extensions;

// boldly borrowed from
// http://blog.peterlesliemorris.com/archive/2011/04/02/a-webserver-for-monotouch-that-also-parses-posted-form-data.aspx

namespace Hadouken.Impl.Http
{
    public class FormData
    {
        readonly NameValueCollection _formValues;
        readonly NameValueCollection FileNames;
        readonly Dictionary<string, MultiPartStreamFileValue> FileObjects;

        public FormData(HttpListenerRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("Request");


            FileNames = new NameValueCollection();
            _formValues = new NameValueCollection();
            FileObjects = new Dictionary<string, MultiPartStreamFileValue>();
            if (request.HasEntityBody)
                ParsePostedValues(request);
        }

        public NameValueCollection FormValues
        {
            get { return _formValues; }
        }

        public MultiPartStreamFileValue GetFile(string name)
        {
            name = FileNames[name];
            if (string.IsNullOrEmpty(name))
                return null;
            MultiPartStreamFileValue result;
            FileObjects.TryGetValue(name, out result);
            return result;
        }

        public IEnumerable<MultiPartStreamFileValue> Files
        {
            get { return FileObjects.Values.ToList().AsReadOnly(); }
        }

        void ParsePostedValues(HttpListenerRequest request)
        {
            Encoding encoding = request.ContentEncoding;

            if (request.ContentType.StartsWith("application/x-www-form-urlencoded"))
                ParseStandardPostValues(request, encoding);
            else
            {
                if (request.ContentType.Length > 20 &&
                        string.Compare(request.ContentType.Substring(0, 20), "multipart/form-data;", true) == 0)
                    ParseMultiPartPostValues(request, encoding);
            }
        }

        void ParseStandardPostValues(HttpListenerRequest request, Encoding encoding)
        {
            using (var reader = new StreamReader(request.InputStream, encoding))
            {
                string postDataString = reader.ReadToEnd();
                string[] lines = postDataString.Split('&');
                foreach (string line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        KeyValuePair<string, string> lineValues = line.ToNameAndValue();
                        AddParsedFormValue(HttpUtility.UrlDecode(lineValues.Key), HttpUtility.UrlDecode(lineValues.Value));
                    }
                }
            }
        }

        void AddParsedFormValue(string key, string value)
        {
            if (_formValues.AllKeys.Contains(key))
                value = _formValues[key] + "," + value;
            _formValues[key] = value;
        }

        void ParseMultiPartPostValues(HttpListenerRequest request, Encoding encoding)
        {
            var reader = new MultiPartStreamReader(request.InputStream, encoding, request.ContentType);
            foreach (MultiPartStreamValue postedValue in reader)
            {
                if (postedValue is MultiPartStreamSimpleValue)
                    AddParsedFormValue(postedValue.Name, (postedValue as MultiPartStreamSimpleValue).Value);
                else
                {
                    var fileValue = (MultiPartStreamFileValue)postedValue;
                    FileNames.Add(fileValue.Name, fileValue.Name);
                    FileObjects.Add(fileValue.Name, fileValue);
                }
            }

        }
    }
}
