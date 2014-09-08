using System;
using System.Management.Automation;
using System.Net.Http;
using System.Net.Http.Headers;
using Hadouken.Tools.Posh.Cmdlets.Models;
using Hadouken.Tools.Posh.Extensions;

namespace Hadouken.Tools.Posh.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Torrents")]
    public sealed class GetTorrents : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public AccessToken AccessToken { get; set; }

        protected override void ProcessRecord()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "Hadouken/Posh 1.0");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AccessToken.Token);

                // Make authenticated request
                var jsonrpc = new
                {
                    id = 1,
                    jsonrpc = "2.0",
                    method = "torrents.getAll",
                };

                var response = client.PostAsJsonAsync(new Uri(AccessToken.Url, "jsonrpc"), jsonrpc).Result;
                var torrents = response.Content.ReadAsJsonRpcAsync<Torrent[]>().Result;
                
                WriteObject(torrents, true);
            }
        }
    }
}
