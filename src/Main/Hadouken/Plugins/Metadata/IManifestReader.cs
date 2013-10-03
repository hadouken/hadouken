using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata
{
    public interface IManifestReader
    {
        Manifest Read(JObject manifestObject);
    }
}
