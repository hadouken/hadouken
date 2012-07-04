using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Hadouken.Data
{
    public interface IMigratorRunner : IComponent
    {
        void Run(Assembly target);
    }
}
