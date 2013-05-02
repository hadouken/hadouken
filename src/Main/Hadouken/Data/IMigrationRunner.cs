using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Hadouken.Data
{
    public interface IMigrationRunner
    {
        void Up(Assembly target);
        void Down(Assembly target);
    }
}
