using System;

namespace Hadouken.Extensions.AutoAdd.Data.Models
{
    public sealed class History
    {
        public History()
        {
            AddedTime = DateTime.UtcNow;
        }

        public int Id { get; set; }

        public string Path { get; set; }

        public DateTime AddedTime { get; set; }
    }
}
