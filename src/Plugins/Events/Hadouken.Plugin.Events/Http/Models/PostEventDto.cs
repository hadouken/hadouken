using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Plugins.Events.Http.Models
{
    public class PostEventDto
    {
        public string Name { get; set; }

        public object Data { get; set; }
    }
}
