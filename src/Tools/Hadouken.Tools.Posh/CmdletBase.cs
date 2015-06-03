using System;
using System.Management.Automation;

namespace Hadouken.Tools.Posh {
    public abstract class CmdletBase : PSCmdlet {
        [Parameter(Mandatory = true)]
        public virtual string AccessToken { get; set; }

        [Parameter(Mandatory = true)]
        public virtual Uri Url { get; set; }
    }
}