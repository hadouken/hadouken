using System;
using System.Management.Automation;
using System.Net.Http;
using System.Net.Http.Headers;
using Hadouken.Tools.Posh.Commands;
using Hadouken.Tools.Posh.Extensions;
using Hadouken.Tools.Posh.Models;

namespace Hadouken.Tools.Posh.Cmdlets
{
    [Cmdlet(VerbsCommon.Remove, "Torrent")]
    public sealed class RemoveTorrentCmdlet : CmdletBase
    {
        private readonly IRemoveTorrentCommand _removeTorrentCommand;

        public RemoveTorrentCmdlet()
            : this(ServiceLocator.Get<IRemoveTorrentCommand>())
        {
            
        }

        internal RemoveTorrentCmdlet(IRemoveTorrentCommand removeTorrentCommand)
        {
            if (removeTorrentCommand == null) throw new ArgumentNullException("removeTorrentCommand");
            _removeTorrentCommand = removeTorrentCommand;
        }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public Torrent Torrent
        {
            get { return _removeTorrentCommand.Torrent; }
            set { _removeTorrentCommand.Torrent = value; }
        }

        [Parameter]
        public bool RemoveData
        {
            get { return _removeTorrentCommand.RemoveData; }
            set { _removeTorrentCommand.RemoveData = value; }
        }

        protected override void ProcessRecord()
        {
            _removeTorrentCommand.Process(new PowershellRuntime(this));
        }
    }
}
