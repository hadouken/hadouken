using Hadouken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HdknPlugins.Rss.Http
{
    public interface IWebClient
    {
        byte[] DownloadData(Uri uri);
    }
}
