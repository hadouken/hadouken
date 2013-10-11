using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Http.Media
{
    public interface IMedia
    {
        string Path { get; set; }
    }

    public class Media : IMedia
    {
        public string Path { get; set; }
    }
}
