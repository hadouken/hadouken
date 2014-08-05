using System.Text.RegularExpressions;
using Hadouken.Common.Extensibility;
using Hadouken.Extensions.Rss.Data.Models;

namespace Hadouken.Extensions.Rss
{
    [Component]
    public class FilterMatcher : IFilterMatcher
    {
        public bool IsMatch(string input, Filter filter)
        {
            if (string.IsNullOrEmpty(filter.IncludePattern)
                    && string.IsNullOrEmpty(filter.ExcludePattern))
            {
                return true;
            }

            if (string.IsNullOrEmpty(filter.IncludePattern)
                && !string.IsNullOrEmpty(filter.ExcludePattern))
            {
                // Only download if exclude pattern doesn't match
                return !Regex.IsMatch(input, filter.ExcludePattern);
            }

            if (!string.IsNullOrEmpty(filter.IncludePattern)
                     && string.IsNullOrEmpty(filter.ExcludePattern))
            {
                // Only download if include pattern matches
                return Regex.IsMatch(input, filter.IncludePattern);
            }

            if (!string.IsNullOrEmpty(filter.IncludePattern)
                     && !string.IsNullOrEmpty(filter.ExcludePattern))
            {
                // Only download if include pattern matches
                // and exclude pattern does not
                return Regex.IsMatch(input, filter.IncludePattern)
                       && !Regex.IsMatch(input, filter.ExcludePattern);
            }

            return false;
        }
    }
}