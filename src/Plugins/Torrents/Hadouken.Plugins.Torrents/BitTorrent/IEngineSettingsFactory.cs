using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Framework.Rpc;
using OctoTorrent.Client;

namespace Hadouken.Plugins.Torrents.BitTorrent
{
    public interface IEngineSettingsFactory
    {
        event EventHandler<EngineSettings> EngineSettingsChanged;

        EngineSettings Build();
    }

    class EngineSettingsFactory : IEngineSettingsFactory
    {
        private static readonly IDictionary<string, Action<EngineSettings, object>> Setters =
            new Dictionary<string, Action<EngineSettings, object>>();

        private static readonly IDictionary<string, Func<EngineSettings, object>> Getters =
            new Dictionary<string, Func<EngineSettings, object>>(); 

        private readonly EngineSettings _settings = new EngineSettings();
        private readonly IJsonRpcClient _rpcClient;

        public event EventHandler<EngineSettings> EngineSettingsChanged;

        static EngineSettingsFactory()
        {
            Setters.Add("bt.downloads.savePath", (settings, o) => settings.SavePath = o.ToStringOrDefault(@"C:\Temporary"));
            Setters.Add("bt.connection.listenPort", (settings, o) => settings.ListenPort = o.ToInt32OrDefault(11001));
            Setters.Add("bt.connection.globalMaxConnections", (settings, o) => settings.GlobalMaxConnections = o.ToInt32OrDefault(150));
            Setters.Add("bt.connection.globalMaxDownloadSpeed", (settings, o) => settings.GlobalMaxDownloadSpeed = o.ToInt32OrDefault(0));
            Setters.Add("bt.connection.globalMaxHalfOpenConnections", (settings, o) => settings.GlobalMaxHalfOpenConnections = o.ToInt32OrDefault(5));
            Setters.Add("bt.connection.globalMaxUploadSpeed", (settings, o) => settings.GlobalMaxUploadSpeed = o.ToInt32OrDefault(0));

            Getters.Add("bt.downloads.savePath", s => s.SavePath);
            Getters.Add("bt.connection.listenPort", s => s.ListenPort);
            Getters.Add("bt.connection.globalMaxConnections", s => s.GlobalMaxConnections);
            Getters.Add("bt.connection.globalMaxDownloadSpeed", s => s.GlobalMaxDownloadSpeed);
            Getters.Add("bt.connection.globalMaxHalfOpenConnections", s => s.GlobalMaxHalfOpenConnections);
            Getters.Add("bt.connection.globalMaxUploadSpeed", s => s.GlobalMaxUploadSpeed);
        }

        public EngineSettingsFactory(IJsonRpcClient rpcClient)
        {
            _rpcClient = rpcClient;
        }

        public EngineSettings Build()
        {
            var keys = Setters.Keys.ToArray();
            var config = _rpcClient.CallAsync<Dictionary<string, object>>("config.getMany", keys).Result;

            foreach (var pair in config)
            {
                if (!Setters.ContainsKey(pair.Key))
                    continue;

                var action = Setters[pair.Key];
                action(_settings, pair.Value);
            }

            // Save on remote end
            var d = Getters.ToDictionary(k => k.Key, v => v.Value(_settings));

            _rpcClient.CallAsync<bool>("config.setMany", d).Wait();

            return _settings;
        }

        protected void OnEngineSettingsChanged(EngineSettings engineSettings)
        {
            var e = EngineSettingsChanged;

            if (e != null)
                e(this, engineSettings);
        }
    }

    public static class ObjectExtensions
    {
        public static string ToStringOrDefault(this object value, string defaultValue)
        {
            if (value == null)
                return defaultValue;

            return value.ToString();
        }

        public static int ToInt32OrDefault(this object value, int defaultValue)
        {
            if (value == null)
                return defaultValue;

            return Convert.ToInt32(value);
        }
    }
}
