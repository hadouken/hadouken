using Hadouken.Extensions.Rss.Config;

namespace Hadouken.Extensions.Rss
{
    public interface IFilterMatcher
    {
        bool IsMatch(string input, Filter filter);
    }
}
