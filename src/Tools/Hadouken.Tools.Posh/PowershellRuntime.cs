using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Hadouken.Tools.Posh {
    public class PowershellRuntime : IRuntime {
        private readonly CmdletBase _cmdletBase;

        public PowershellRuntime(CmdletBase cmdletBase) {
            if (cmdletBase == null) {
                throw new ArgumentNullException("cmdletBase");
            }
            this._cmdletBase = cmdletBase;
        }

        public string AccessToken {
            get { return this._cmdletBase.AccessToken; }
        }

        public Uri Url {
            get { return this._cmdletBase.Url; }
        }

        public ICollection<string> GetResolvedPaths(string path) {
            ProviderInfo provider;
            return this._cmdletBase.GetResolvedProviderPathFromPSPath(path, out provider);
        }

        public void WriteObject(object obj, bool enumerateCollection) {
            this._cmdletBase.WriteObject(obj, enumerateCollection);
        }
    }
}