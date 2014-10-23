using System;
using System.Management.Automation;
using Hadouken.Tools.Posh.Commands;

namespace Hadouken.Tools.Posh.Cmdlets
{
    [Cmdlet(VerbsCommon.Add, "Torrent")]
    public sealed class AddTorrentCmdlet : CmdletBase
    {
        private readonly IAddTorrentCommand _addTorrentCommand;

        public AddTorrentCmdlet()
            : this(ServiceLocator.Get<IAddTorrentCommand>())
        {
        }

        internal AddTorrentCmdlet(IAddTorrentCommand addTorrentCommand)
        {
            if (addTorrentCommand == null) throw new ArgumentNullException("addTorrentCommand");
            _addTorrentCommand = addTorrentCommand;
        }

        [Parameter(Mandatory = true)]
        public string Path
        {
            get { return _addTorrentCommand.Path; }
            set { _addTorrentCommand.Path = value; }
        }

        [Parameter]
        public string SavePath
        {
            get { return _addTorrentCommand.SavePath; }
            set { _addTorrentCommand.SavePath = value; }
        }

        protected override void ProcessRecord()
        {
            _addTorrentCommand.Process(new PowershellRuntime(this));
        }
    }
}
