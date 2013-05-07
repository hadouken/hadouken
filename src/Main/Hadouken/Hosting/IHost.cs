using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Hosting
{
    public interface IHost : IComponent
    {
        void Load();
        void Unload();
    }
}
