using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Hadouken.Data
{
    public interface IMigrationRunner : IComponent
    {
        void Up(Assembly target);
        void Down(Assembly target);
    }
}
