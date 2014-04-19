using System.Collections.Generic;

namespace Hadouken.Plugins.Metadata
{
    public interface IPage
    {
        IEnumerable<string> Scripts { get; }

        string HtmlFile { get; }
    }
}