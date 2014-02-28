using System;
using System.IO;
using System.Threading.Tasks;

namespace Hadouken.Http
{
    public interface IApiConnection
    {
        Task<T> GetAsync<T>(Uri uri) where T : class;

        byte[] DownloadData(Uri uri);
    }
}
