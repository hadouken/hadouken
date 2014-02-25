using System;
using System.Linq;

namespace Hadouken.SemVer
{
    public class SemanticVersionRange
    {
        private readonly Rule[] _rules;

        protected SemanticVersionRange() {}

        protected SemanticVersionRange(params Rule[] rules)
        {
            _rules = rules;
        }

        public Rule[] Rules
        {
            get { return _rules; }
        }

        public bool IsIncluded(SemanticVersion version)
        {
            if (_rules == null)
                return false;

            return _rules.All(r => r.IsIncluded(version));
        }

        public static bool TryParse(string versionRange, out SemanticVersionRange range)
        {
            Exception exception;
            return TryParse(versionRange, out range, out exception);
        }

        public static bool TryParse(string versionRange, out SemanticVersionRange range, out Exception exception)
        {
            range = null;
            exception = null;

            try
            {
                // empty = latest version.
                if (String.IsNullOrEmpty(versionRange))
                {
                    range = new SemanticVersionRange();
                    return true;
                }

                // if it starts with a numeric character it should be a single version
                // 1.0 = 1.0 <= x
                if (Char.IsNumber(versionRange[0]))
                {
                    var version = new SemanticVersion(versionRange);
                    range = new SemanticVersionRange(new GreaterThanOrEqualsRule(version));
                    return true;
                }

                var isRange = versionRange.Contains(",");
                var lowerInclusive = versionRange[0] == '[';
                var upperInclusive = versionRange[versionRange.Length - 1] == ']';

                // (1.0) = invalid
                if (!isRange && !lowerInclusive && !upperInclusive)
                    return false;

                // [1.0] = x == 1.0
                if (!isRange && lowerInclusive && upperInclusive)
                {
                    var versionString = versionRange.Substring(1, versionRange.Length - 2);
                    var version = new SemanticVersion(versionString);

                    range = new SemanticVersionRange(new EqualsRule(version));
                    return true;
                }

                var rangeParts = versionRange.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                // check if we have an invalid range, eg. ",1.0" or [1.0,"
                // if we get here, we should only have ranges which contains a comma and either
                // [ or ( in the beginning and ) or ] in the end.
                if (rangeParts.Length != 2)
                    return false;

                // Skip first character to get just the version
                var lowerVersion = rangeParts[0].Substring(1);
                // Skip last character to get just the version
                var upperVersion = rangeParts[1].Substring(0, rangeParts[1].Length - 1);

                // Both versions cannot be null or empty
                if (String.IsNullOrEmpty(lowerVersion) && String.IsNullOrEmpty(upperVersion))
                    return false;

                if (String.IsNullOrEmpty(lowerVersion))
                {
                    Rule rule;

                    if (upperInclusive)
                    {
                        rule = new LessThanOrEqualsRule(upperVersion);
                    }
                    else
                    {
                        rule = new LessThanRule(upperVersion);
                    }

                    range = new SemanticVersionRange(rule);
                    return true;
                }

                if (String.IsNullOrEmpty(upperVersion))
                {
                    Rule rule;

                    if (lowerInclusive)
                    {
                        rule = new GreaterThanOrEqualsRule(lowerVersion);
                    }
                    else
                    {
                        rule = new GreaterThanRule(lowerVersion);
                    }

                    range = new SemanticVersionRange(rule);
                    return true;
                }

                Rule upperRule;
                Rule lowerRule;

                if (upperInclusive)
                {
                    upperRule = new LessThanOrEqualsRule(upperVersion);
                }
                else
                {
                    upperRule = new LessThanRule(upperVersion);
                }

                if (lowerInclusive)
                {
                    lowerRule = new GreaterThanOrEqualsRule(lowerVersion);
                }
                else
                {
                    lowerRule = new GreaterThanRule(lowerVersion);
                }

                range = new SemanticVersionRange(lowerRule, upperRule);
                return true;
            }
            catch (Exception e)
            {
                range = null;
                exception = e;

                return false;
            }
        }
    }
}
