namespace Hadouken.Extensions.Rss.Config
{
    public sealed class Filter
    {
        public string IncludePattern { get; set; }

        public string ExcludePattern { get; set; }

        public bool AutoStart { get; set; }

        public Modifier[] Modifiers { get; set; }
    }
}
