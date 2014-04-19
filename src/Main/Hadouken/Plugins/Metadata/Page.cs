using System.Collections.Generic;
using System.Linq;

namespace Hadouken.Plugins.Metadata
{
    public sealed class Page : IPage
    {
        private readonly string _htmlFile;
        private readonly IEnumerable<string> _scripts;

        public Page(string htmlFile, IEnumerable<string> scripts)
        {
            _htmlFile = htmlFile;
            _scripts = scripts ?? Enumerable.Empty<string>();
        }

        public IEnumerable<string> Scripts
        {
            get { return _scripts; }
        }

        public string HtmlFile
        {
            get { return _htmlFile; }
        }
    }
}