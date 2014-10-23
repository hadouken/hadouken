using System;
using System.IO;
using System.Management.Automation;
using System.Net.Http;
using System.Net.Http.Headers;
using Hadouken.Tools.Posh.Cmdlets.Models;
using Hadouken.Tools.Posh.Extensions;

namespace Hadouken.Tools.Posh.Cmdlets
{
    [Cmdlet(VerbsCommon.Add, "Torrent")]
    public sealed class AddTorrent : Cmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public AccessToken AccessToken { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        public string Path { get; set; }

        [Parameter(Mandatory = true, Position = 2)]
        public string SavePath { get; set; }

        protected override void ProcessRecord()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "Hadouken/Posh 1.0");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AccessToken.Token);

                // Make authenticated request
                var file = File.ReadAllBytes(Path);
                var data = Convert.ToBase64String(file);

                var jsonrpc = new
                {
                    id = 1,
                    jsonrpc = "2.0",
                    method = "torrents.addFile",
                    @params = new object[] { data, SavePath, "" }
                };

                var response = client.PostAsJsonAsync(new Uri(AccessToken.Url, "jsonrpc"), jsonrpc).Result;
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
