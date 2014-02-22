using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hadouken.Http
{
    public class ApiConnection : IApiConnection
    {
        public async Task<T> GetAsync<T>(Uri uri) where T : class, new()
        {
            using (var client = new HttpClient())
            {
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