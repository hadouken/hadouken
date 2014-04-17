using System.IO;

namespace Hadouken.Web
{
    public static class WebPage
    {
        public static byte[] GetLoginPage()
        {
            using (var stream = typeof (WebPage).Assembly.GetManifestResourceStream("Hadouken.Web.login.html"))
            using (var ms = new MemoryStream())
            {
                if (stream != null) stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
