using System;
using System.Management.Automation;
using Hadouken.Tools.Posh.Commands;
using Hadouken.Tools.Posh.Models;

namespace Hadouken.Tools.Posh.Cmdlets {
    [Cmdlet(VerbsCommon.Remove, "Torrent")]
    public sealed class RemoveTorrentCmdlet : CmdletBase {
        private readonly IRemoveTorrentCommand _removeTorrentCommand;

        public RemoveTorrentCmdlet()
            : this(ServiceLocator.Get<IRemoveTorrentCommand>()) {}

        internal RemoveTorrentCmdlet(IRemoveTorrentCommand removeTorrentCommand) {
            if (removeTorrentCommand == null) {
                throw new ArgumentNullException("removeTorrentCommand");
            }
            this._removeTorrentCommand = removeTorrentCommand;
        }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public Torrent Torrent {
            get { return this._removeTorrentCommand.Torrent; }
            set { this._removeTorrentCommand.Torrent = value; }
        }

        [Parameter]
        public bool RemoveData {
            get { return this._removeTorrentCommand.RemoveData; }
            set { this._removeTorrentCommand.RemoveData = value; }
        }

        protected override void ProcessRecord() {
            this._removeTorrentCommand.Process(new PowershellRuntime(this));
        }
    }
}