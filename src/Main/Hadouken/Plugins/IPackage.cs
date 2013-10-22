using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.SemVer;
using Hadouken.Plugins.Metadata;

namespace Hadouken.Plugins
{
    public interface IPackage
    {
        IManifest Manifest { get; }


    }
}
