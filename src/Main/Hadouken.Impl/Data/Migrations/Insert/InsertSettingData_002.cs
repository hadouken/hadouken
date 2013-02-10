using Hadouken.Security;
using Migrator.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Hadouken.Impl.Data.Migrations.Insert
{
    /// <summary>
    /// This migration adds the keys auth.username and auth.password to the database since we don't want them in the App.config no mo'
    /// </summary>
    [Migration(20130210130215)]
    public class InsertSettingData_002 : Migration
    {
        private static readonly string DefaultPassword = Hash.Generate("hdkn");

        public override void Down()
        {
        }

        public override void Up()
        {
            Database.Insert("Setting", new[] { "Key", "Value", "Type", "Permissions", "Options" }, new[] { "auth.username", "\"hdkn\"", "System.String", "3", "0" });
            Database.Insert("Setting", new[] { "Key", "Value", "Type", "Permissions", "Options" }, new[] { "auth.password", "\"" + DefaultPassword + "\"", "System.String", "3", "1" });
        }
    }
}
