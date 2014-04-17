using System.Collections.Generic;

namespace Hadouken.Plugins.Metadata
{
    public interface ISettingsPage
    {
        IEnumerable<string> Scripts { get; }

        string HtmlFile { get; }
    }
}