using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Hadouken.Extensions
{
    public static class StringExtensions
    {
        public static KeyValuePair<string, string> ToNameAndValue(this string instance, char separator = '=')
        {
            if (!instance.Contains(separator))
                throw new InvalidOperationException("Invalid key/value pair");
            int index = instance.IndexOf(separator);
            string name = instance.Substring(0, index).Trim();
            string value = instance.Substring(index + 1).Trim();
            return new KeyValuePair<string, string>(name, value);
        }

        public static NameValueCollection ToNameValueCollection(this string instance, char separator = ';')
        {
            string[] namesAndValues = instance.Split(separator);
            var result = new NameValueCollection(namesAndValues.Length);
            foreach (string line in namesAndValues)
            {
                var nameAndValue = line.ToNameAndValue();
                result.Add(nameAndValue.Key, nameAndValue.Value);
            }
            return result;
        }

        public static string UnQuote(this string instance)
        {
            if (instance == null)
                throw new ArgumentNullException("Instance");
            if (!instance.StartsWith("\""))
                throw new ArgumentException("Value does not start with a quote: " + instance);
            if (!instance.EndsWith("\""))
                throw new ArgumentException("Value does not end with a quote: " + instance);

            return instance.Substring(1, instance.Length - 2);
        }
    }
}
