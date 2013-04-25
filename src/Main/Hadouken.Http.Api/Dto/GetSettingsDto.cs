using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http.Api.Dto
{
    public class GetSettingsDto
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public int Type { get; set; }
        public int Permissions { get; set; }
        public int Options { get; set; }
    }
}
