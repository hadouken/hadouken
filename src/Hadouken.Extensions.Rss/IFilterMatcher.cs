using Hadouken.Extensions.Rss.Data.Models;

namespace Hadouken.Extensions.Rss
{
    public interface IFilterMatcher
    {
        bool IsMatch(string input, Filter filter);
    }
}
