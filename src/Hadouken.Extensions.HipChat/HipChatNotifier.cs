using System;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Extensions.HipChat.Config;
using Hadouken.Extensions.HipChat.Http;

namespace Hadouken.Extensions.HipChat
{
    [Extension("notifier.hipchat",
        Name = "HipChat",
        Description = "Sends notifications to a HipChat room."
    )]
    public class HipChatNotifier : INotifier
    {
        private readonly IKeyValueStore _keyValueStore;
        private readonly IHipChatClient _hipChatClient;

        public HipChatNotifier(IKeyValueStore keyValueStore,
            IHipChatClient hipChatClient)
        {
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            if (hipChatClient == null) throw new ArgumentNullException("hipChatClient");
            _keyValueStore = keyValueStore;
            _hipChatClient = hipChatClient;
        }

        public bool CanNotify()
        {
            var config = GetConfig();
            return (config != null
                    && !string.IsNullOrEmpty(config.AuthenticationToken)
                    && !string.IsNullOrEmpty(config.RoomId)
                    && !string.IsNullOrEmpty(config.From));
        }

        public void Notify(Notification notification)
        {
            var config = GetConfig();
            _hipChatClient.SendMessage(config, notification.Message);
        }

        private HipChatConfig GetConfig()
        {
            return _keyValueStore.Get<HipChatConfig>("hipchat.config");
        }
    }
}
