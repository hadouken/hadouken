using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Extensions.AutoAdd.Data.Models;

namespace Hadouken.Extensions.AutoAdd.Data {
    [Component]
    public class AutoAddRepository : IAutoAddRepository {
        private readonly IDbConnection _connection;

        public AutoAddRepository(IDbConnection connection) {
            if (connection == null) {
                throw new ArgumentNullException("connection");
            }
            this._connection = connection;
        }

        public void CreateFolder(Folder folder) {
            var query =
                @"insert into AutoAdd_Folder (Path, Pattern, RemoveSourceFile, RecursiveSearch, AutoStart, Label) values (@Path, @Pattern, @RemoveSourceFile, @RecursiveSearch, @AutoStart, @Label); select last_insert_rowid();";
            folder.Id = this._connection.Query<int>(query, folder).First();
        }

        public void CreateHistory(History history) {
            var query =
                @"insert into AutoAdd_History (Path, AddedTime) values (@Path, @AddedTime); select last_insert_rowid();";
            history.Id = this._connection.Query<int>(query, history).First();
        }

        public void DeleteFolder(int folderId) {
            var query = @"delete from AutoAdd_Folder where Id = @Id";
            this._connection.Execute(query, new {Id = folderId});
        }

        public IEnumerable<Folder> GetFolders() {
            var query =
                @"select f.Id, f.Path, f.Pattern, f.RemoveSourceFile, f.AutoStart, f.Label from AutoAdd_Folder f";
            return this._connection.Query<Folder>(query);
        }

        public History GetHistoryByPath(string path) {
            var query = @"select h.Id, h.Path, h.AddedTime from AutoAdd_History h where h.Path = @Path limit 1";
            return this._connection.Query<History>(query, new {Path = path}).FirstOrDefault();
        }

        public void UpdateFolder(Folder folder) {
            var query =
                @"update AutoAdd_Folder set Path = @Path, Pattern = @Pattern, RemoveSourceFile = @RemoveSourceFile, RecursiveSearch = @RecursiveSearch, AutoStart = @AutoStart, Label = @Label where Id = @Id";
            this._connection.Execute(query, folder);
        }
    }
}