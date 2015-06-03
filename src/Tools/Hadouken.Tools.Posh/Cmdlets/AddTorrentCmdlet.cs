using System;
using System.Management.Automation;
using Hadouken.Tools.Posh.Commands;

namespace Hadouken.Tools.Posh.Cmdlets {
    [Cmdlet(VerbsCommon.Add, "Torrent")]
    public sealed class AddTorrentCmdlet : CmdletBase {
        private readonly IAddTorrentCommand _addTorrentCommand;

        public AddTorrentCmdlet()
            : this(ServiceLocator.Get<IAddTorrentCommand>()) {}

        internal AddTorrentCmdlet(IAddTorrentCommand addTorrentCommand) {
            if (addTorrentCommand == null) {
                throw new ArgumentNullException("addTorrentCommand");
            }
            this._addTorrentCommand = addTorrentCommand;
        }

        [Parameter(Mandatory = true)]
        public string Path {
            get { return this._addTorrentCommand.Path; }
            set { this._addTorrentCommand.Path = value; }
        }

        [Parameter]
        public string SavePath {
            get { return this._addTorrentCommand.SavePath; }
            set { this._addTorrentCommand.SavePath = value; }
        }

        protected override void ProcessRecord() {
            this._addTorrentCommand.Process(new PowershellRuntime(this));
        }
    }
}