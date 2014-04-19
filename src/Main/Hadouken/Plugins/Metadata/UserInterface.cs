using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hadouken.Plugins.Metadata
{
    public sealed class UserInterface : IUserInterface
    {
        private readonly IEnumerable<string> _backgroundScripts;
        private readonly IDictionary<string, IPage> _pages;

        public UserInterface()
        {
            _backgroundScripts = Enumerable.Empty<string>();
            _pages = new ReadOnlyDictionary<string, IPage>(null);
        }

        public UserInterface(IEnumerable<string> backgroundScripts, IDictionary<string, IPage> pages)
        {
            _backgroundScripts = backgroundScripts;
            _pages = pages;
        }

        public IEnumerable<string> BackgroundScripts
        {
            get { return _backgroundScripts; }
        }

        public IDictionary<string, IPage> Pages
        {
            get { return _pages; }
        }
    }
}
