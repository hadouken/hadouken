using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Hadouken;

namespace HdknPlugins.Rss.Http
{
    [Component]
    public class DefaultWebClient : IWebClient
    {
        private readonly WebClient _webClient = new WebClient();

        public byte[] DownloadData(Uri uri)
        {
            return _webClient.DownloadData(uri);
        }
    }
}
