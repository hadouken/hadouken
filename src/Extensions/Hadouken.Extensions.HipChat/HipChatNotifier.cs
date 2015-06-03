using System;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Extensions.HipChat.Config;
using Hadouken.Extensions.HipChat.Http;

namespace Hadouken.Extensions.HipChat {
    [Extension("notifier.hipchat",
        Name = "HipChat",
        Description = "Sends notifications to a HipChat room."
        )]
    [Configuration(typeof (HipChatConfig), Key = "hipchat.config")]
    public class HipChatNotifier : INotifier {
        private readonly IHipChatClient _hipChatClient;
        private readonly IKeyValueStore _keyValueStore;

        public HipChatNotifier(IKeyValueStore keyValueStore,
            IHipChatClient hipChatClient) {
            if (keyValueStore == null) {
                throw new ArgumentNullException("keyValueStore");
            }
            if (hipChatClient == null) {
                throw new ArgumentNullException("hipChatClient");
            }
            this._keyValueStore = keyValueStore;
            this._hipChatClient = hipChatClient;
        }

        public bool CanNotify() {
            var config = this.GetConfig();
            return (config != null
                    && !string.IsNullOrEmpty(config.AuthenticationToken)
                    && !string.IsNullOrEmpty(config.RoomId)
                    && !string.IsNullOrEmpty(config.From));
        }

        public void Notify(Notification notification) {
            var config = this.GetConfig();
            this._hipChatClient.SendMessage(config, notification.Message);
        }

        private HipChatConfig GetConfig() {
            return this._keyValueStore.Get<HipChatConfig>("hipchat.config");
        }
    }
}