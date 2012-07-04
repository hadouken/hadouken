using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.Data;
using Hadouken.Data.Models;

namespace Hadouken.Data
{
    public static class DataRepositoryExtensions
    {
        public static string GetSetting(this IDataRepository data, string key, string defaultValue)
        {
            var setting = data.Single<Setting>(s => s.Key == key);

            if (setting == null)
            {
                setting = new Setting();
                setting.Key = key;
                setting.Value = defaultValue;

                data.Save(setting);
            }

            return setting.Value;
        }
    }
}
