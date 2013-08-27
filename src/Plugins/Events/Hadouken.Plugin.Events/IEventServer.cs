using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Plugins.Events
{
    public interface IEventServer
    {
        void Start();

        void Stop();
    }
}
