using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using Hadouken.Tools.Posh.Cmdlets.Models;
using Hadouken.Tools.Posh.Extensions;

namespace Hadouken.Tools.Posh.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "HadoukenTorrents")]
    public class GetHadoukenTorrents : PSCmdlet
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

                var loginForm = new {UserName, Password = Password.ToUnsecureString()};
                var loginResponse = client.PostAsJsonAsync(new Uri(Url, "auth/login"), loginForm).Result;
                var data = loginResponse.Content.ReadAsJsonAsync<IDictionary<string, object>>().Result;

                var token = data["token"].ToString();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token);

                // Make authenticated request
                var jsonrpc = new
                {
                    id = 1,
                    jsonrpc = "2.0",
                    method = "torrents.getAll",
                };

                var response = client.PostAsJsonAsync(new Uri(Url, "jsonrpc"), jsonrpc).Result;
                var torrents = response.Content.ReadAsJsonRpcAsync<Torrent[]>().Result;
                
                WriteObject(torrents, true);
            }
        }
    }
}
