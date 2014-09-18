using System;
using Hadouken.Common.Logging;
using Hadouken.Common.Timers;
using Hadouken.Extensions.AutoAdd.Data;
using NSubstitute;

namespace Hadouken.Extensions.AutoAdd.Tests.Fixtures
{
    internal sealed class AutoAddPluginFixture
    {
        public AutoAddPluginFixture()
        {
            Logger = Substitute.For<ILogger<AutoAddPlugin>>();

            Timer = Substitute.For<ITimer>();
            TimerFactory = Substitute.For<ITimerFactory>();
            TimerFactory.Create(Arg.Any<int>(), Arg.Any<Action>()).Returns(Timer);

            AutoAddRepository = Substitute.For<IAutoAddRepository>();
            FolderScanner = Substitute.For<IFolderScanner>();
        }

        public ILogger<AutoAddPlugin> Logger { get; set; }

        public ITimerFactory TimerFactory { get; set; }

        public ITimer Timer { get; set; }

        public IAutoAddRepository AutoAddRepository { get; set; }

        public IFolderScanner FolderScanner { get; set; }

        public AutoAddPlugin CreateAutoAddPlugin()
        {
            return new AutoAddPlugin(Logger, TimerFactory, AutoAddRepository, FolderScanner);
        }
    }
}
