using System;

namespace Hadouken.Extensions.Kodi.Config
{
    public sealed class KodiConfig
    {
        public Uri Url { get; set; }

        public bool EnableAuthentication { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
