using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;
using Hadouken.Common.Data;
using Hadouken.Data.Models;
using Hadouken.Http.Api.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Hadouken.Configuration;

namespace Hadouken.Http.Api
{
    public class SettingsController : ApiController
    {
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();

        private readonly IDataRepository _repository;
        private readonly IKeyValueStore _keyValueStore;

        public SettingsController(IDataRepository repository, IKeyValueStore keyValueStore)
        {
            if(repository == null)
                throw new ArgumentNullException("repository");

            if(keyValueStore == null)
                throw new ArgumentNullException("keyValueStore");

            _repository = repository;
            _keyValueStore = keyValueStore;
        }

        public IEnumerable<GetSettingsDto> Get()
        {
            return (from setting in _repository.List<Setting>()
                    where setting.Key != "auth.username"
                    select new GetSettingsDto
                        {
                            Key = setting.Key,
                            Value = _serializer.DeserializeObject(setting.Value),
                            Type = GetSettingType(setting.Type),
                            Options = (int)setting.Permissions,
                            Permissions = (int)setting.Permissions
                        });
        } 

        public HttpResponseMessage Post([FromBody] Dictionary<string, object> data)
        {
            foreach (var key in data.Keys)
            {
                // ugly fix for newtonsoft objects
                var temp = Newtonsoft.Json.JsonConvert.SerializeObject(data[key]);
                _keyValueStore.Set(key, _serializer.DeserializeObject(temp));
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        private static int GetSettingType(string type)
        {
            switch (type)
            {
                case "System.Boolean":
                    return 0;

                case "System.Int32":
                    return 1;

                case "System.String":
                    return 2;

                default:
                    return 99;
            }
        }
    }
}
