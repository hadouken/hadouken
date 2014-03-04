using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hadouken.Http
{
    public class ApiConnection : IApiConnection
    {
        private static readonly string VersionHeaderKey = "X-Hadouken-Version";
        private readonly string _version;

        public ApiConnection()
        {
            var type = typeof (ApiConnection);
            var infoVersion = type.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            if (infoVersion != null)
            {
                _version = infoVersion.InformationalVersion;
            }
        }

        public async Task<T> GetAsync<T>(Uri uri) where T : class
        {
            using (var client = new HttpClient())
            {
                if (string.IsNullOrEmpty(_version))
                {
                    // Add the Hadouken version to headers
                    client.DefaultRequestHeaders.Add(VersionHeaderKey, _version);
                }

                var response = await client.GetAsync(uri);
                
                if(!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(content);
            }
        }

        public byte[] DownloadData(Uri uri)
        {
            return new WebClient().DownloadData(uri);
        }
    }
}