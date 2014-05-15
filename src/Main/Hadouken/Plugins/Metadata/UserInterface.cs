using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hadouken.Plugins.Metadata
{
    public sealed class UserInterface
    {
        private readonly IEnumerable<string> _backgroundScripts;
        private readonly IDictionary<string, Page> _pages;

        public UserInterface()
        {
            _backgroundScripts = Enumerable.Empty<string>();
            _pages = new ReadOnlyDictionary<string, Page>(new Dictionary<string, Page>());
        }

        public UserInterface(IEnumerable<string> backgroundScripts, IDictionary<string, Page> pages)
        {
            _backgroundScripts = backgroundScripts;
            _pages = pages;
        }

        public IEnumerable<string> BackgroundScripts
        {
            get { return _backgroundScripts; }
        }

        public IDictionary<string, Page> Pages
        {
            get { return _pages; }
        }
    }
}
