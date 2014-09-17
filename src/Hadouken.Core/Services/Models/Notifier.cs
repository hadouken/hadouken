using System.Collections.Generic;

namespace Hadouken.Core.Services.Models
{
    public sealed class Notifier
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool CanNotify { get; set; }

        public IEnumerable<string> RegisteredTypes { get; set; }
    }
}
