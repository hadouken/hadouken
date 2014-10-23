using System;
using System.Management.Automation;
using Hadouken.Tools.Posh.Commands;

namespace Hadouken.Tools.Posh.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Torrent")]
    public sealed class GetTorrentCmdlet : CmdletBase
    {
        private readonly IGetTorrentCommand _getTorrentCommand;

        public GetTorrentCmdlet()
            : this(ServiceLocator.Get<IGetTorrentCommand>())
        {
        }

        internal GetTorrentCmdlet(IGetTorrentCommand getTorrentCommand)
        {
            if (getTorrentCommand == null) throw new ArgumentNullException("getTorrentCommand");
            _getTorrentCommand = getTorrentCommand;
        }

        protected override void ProcessRecord()
        {
            _getTorrentCommand.Process(new PowershellRuntime(this));
        }
    }
}
