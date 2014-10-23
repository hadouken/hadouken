using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using Hadouken.Tools.Posh.Cmdlets.Models;
using Hadouken.Tools.Posh.Extensions;

namespace Hadouken.Tools.Posh.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "AccessToken")]
    public sealed class GetAccessToken : Cmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public Uri Url { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        public string UserName { get; set; }

        [Parameter(Mandatory = true, Position = 2)]
        public SecureString Password { get; set; }

        protected override void ProcessRecord()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "Hadouken/Posh 1.0");

                var loginForm = new { UserName, Password = Password.ToUnsecureString() };
                var loginResponse = client.PostAsJsonAsync(new Uri(Url, "auth/login"), loginForm).Result;
                var data = loginResponse.Content.ReadAsJsonAsync<IDictionary<string, object>>().Result;

                var accessToken = new AccessToken
                {
                    Url = Url,
                    Token = data["token"].ToString()
                };

                WriteObject(accessToken);
            }
        }
    }
}
