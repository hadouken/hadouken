namespace Hadouken.Extensions.Rss.Data.Models
{
    public sealed class Modifier
    {
        public int Id { get; set; }

        public int FilterId { get; set; }

        public ModifierTarget Target { get; set; }

        public string Value { get; set; }
    }
}
