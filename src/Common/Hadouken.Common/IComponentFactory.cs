using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common
{
    public interface IComponentFactory<out TComponent>
    {
        TComponent Create();
    }
}
