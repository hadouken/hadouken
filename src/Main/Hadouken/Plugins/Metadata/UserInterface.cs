using System.Collections.Generic;
using System.Linq;

namespace Hadouken.Plugins.Metadata
{
    public sealed class UserInterface : IUserInterface
    {
        private readonly IEnumerable<string> _backgroundScripts;
        private readonly ISettingsPage _settingsPage;

        public UserInterface()
        {
            _backgroundScripts = Enumerable.Empty<string>();
            _settingsPage = null;
        }

        public UserInterface(IEnumerable<string> backgroundScripts, ISettingsPage settingsPage)
        {
            _backgroundScripts = backgroundScripts;
            _settingsPage = settingsPage;
        }

        public IEnumerable<string> BackgroundScripts
        {
            get { return _backgroundScripts; }
        }

        public ISettingsPage SettingsPage
        {
            get { return _settingsPage; }
        }
    }
}
