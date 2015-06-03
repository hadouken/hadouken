namespace Hadouken.Extensions.Rss.Data.Models {
    public sealed class Filter {
        public int Id { get; set; }
        public int FeedId { get; set; }
        public string IncludePattern { get; set; }
        public string ExcludePattern { get; set; }
        public bool AutoStart { get; set; }
    }
}