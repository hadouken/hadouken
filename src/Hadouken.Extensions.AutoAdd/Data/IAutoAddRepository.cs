using System.Collections.Generic;
using Hadouken.Extensions.AutoAdd.Data.Models;

namespace Hadouken.Extensions.AutoAdd.Data
{
    public interface IAutoAddRepository
    {
        void CreateFolder(Folder folder);

        void CreateHistory(History history);

        void DeleteFolder(int folderId);

        IEnumerable<Folder> GetFolders();

        History GetHistoryByPath(string path);

        void UpdateFolder(Folder folder);
    }
}
