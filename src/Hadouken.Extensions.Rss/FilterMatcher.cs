using Hadouken.Common.Extensibility;
using Hadouken.Extensions.Rss.Data.Models;

namespace Hadouken.Extensions.Rss
{
    [Component]
    public class FilterMatcher : IFilterMatcher
    {
        public bool IsMatch(string input, Filter filter)
        {
            throw new System.NotImplementedException();
        }
    }
}