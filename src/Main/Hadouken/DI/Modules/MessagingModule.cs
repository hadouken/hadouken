using Autofac;
using Hadouken.Messaging;
using Hadouken.Plugins.Handlers;
using Hadouken.Plugins.Messages;

namespace Hadouken.DI.Modules
{
    public sealed class MessagingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MessageQueue>().As<IMessageQueue>();
            builder.RegisterType<PluginErrorHandler>().As<IMessageHandler<PluginErrorMessage>>();
        }
    }
}
