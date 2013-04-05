using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.Plugins;
using Hadouken.Common.Messaging;
using Hadouken.Common.Data;

using HdknPlugins.AutoAdd.Timers;
using HdknPlugins.AutoAdd.Data.Models;

namespace HdknPlugins.AutoAdd
{
    public class AutoAddPlugin : Plugin
    {
        private readonly IDataRepository _dataRepository;
        private readonly ITimer _timer;

        public AutoAddPlugin(IMessageBus messageBus, IDataRepository dataRepository, ITimerFactory timerFactory)
            : base(messageBus)
        {
            _dataRepository = dataRepository;
            _timer = timerFactory.CreateTimer();
        }

        public override void Load()
        {
            _timer.SetCallback(1000, CheckFolders);
            _timer.Start();
        }

        private void CheckFolders()
        {
            var folders = _dataRepository.List<Folder>();
        }

        public override void Unload()
        {
            _timer.Stop();
        }
    }
}
