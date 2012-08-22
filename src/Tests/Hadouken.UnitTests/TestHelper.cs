using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Hadouken.UnitTests
{
    public static class TestHelper
    {
        public static MemoryStream LoadResource(string resourceName)
        {
            var ms = new MemoryStream();

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                stream.CopyTo(ms);
                return ms;
            }
        }
    }
}
