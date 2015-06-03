using System;
using Hadouken.Common.Logging;
using Hadouken.Common.Timers;
using Hadouken.Extensions.AutoAdd.Data;
using NSubstitute;

namespace Hadouken.Extensions.AutoAdd.Tests.Fixtures {
    internal sealed class AutoAddPluginFixture {
        public AutoAddPluginFixture() {
            this.Logger = Substitute.For<ILogger<AutoAddPlugin>>();

            this.Timer = Substitute.For<ITimer>();
            this.TimerFactory = Substitute.For<ITimerFactory>();
            this.TimerFactory.Create(Arg.Any<int>(), Arg.Any<Action>()).Returns(this.Timer);

            this.AutoAddRepository = Substitute.For<IAutoAddRepository>();
            this.FolderScanner = Substitute.For<IFolderScanner>();
        }

        public ILogger<AutoAddPlugin> Logger { get; set; }
        public ITimerFactory TimerFactory { get; set; }
        public ITimer Timer { get; set; }
        public IAutoAddRepository AutoAddRepository { get; set; }
        public IFolderScanner FolderScanner { get; set; }

        public AutoAddPlugin CreateAutoAddPlugin() {
            return new AutoAddPlugin(this.Logger, this.TimerFactory, this.AutoAddRepository, this.FolderScanner);
        }
    }
}