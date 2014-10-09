namespace Hadouken.Extensions.AutoAdd.Data.Models
{
    public sealed class Folder
    {
        public int Id { get; set; }

        public string Path { get; set; }

        public string Pattern { get; set; }

        public bool RemoveSourceFile { get; set; }

        public bool RecursiveSearch { get; set; }

        public bool AutoStart { get; set; }
    }
}
