using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Plugins.Metadata
{
    public class SemanticVersion
    {
        private int _major;
        private int _minor;
        private int _patch;

        public SemanticVersion() : this(0, 0, 0, String.Empty) {}

        public SemanticVersion(string semver)
        {
            ParseFromString(semver);
        }

        public SemanticVersion(int major) : this(major, 0, 0, String.Empty) {}

        public SemanticVersion(int major, int minor) : this(major, minor, 0, String.Empty) {}

        public SemanticVersion(int major, int minor, int patch) : this(major, minor, patch, String.Empty) {}

        public SemanticVersion(int major, int minor, int patch, string label)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Label = label;
        }

        public int Major
        {
            get { return _major; }
            set
            {
                ThrowIfNegative(value);
                _major = value;
            }
        }

        public int Minor
        {
            get { return _minor; }
            set
            {
                ThrowIfNegative(value);
                _minor = value;
            }
        }

        public int Patch
        {
            get { return _patch; }
            set
            {
                ThrowIfNegative(value);
                _patch = value;
            }
        }

        public string Label { get; set; }

        public static bool operator <(SemanticVersion x, SemanticVersion y)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            if (x.Major < y.Major)
                return true;

            if (x.Minor < y.Minor)
                return true;

            if (x.Patch < y.Patch)
                return true;

            return String.Compare(x.Label, y.Label, StringComparison.Ordinal) < 0;
        }

        public static bool operator >(SemanticVersion x, SemanticVersion y)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            if (x.Major > y.Major)
                return true;

            if (x.Minor > y.Minor)
                return true;

            if (x.Patch > y.Patch)
                return true;

            return String.Compare(x.Label, y.Label, StringComparison.Ordinal) > 0;
        }

        public static implicit operator SemanticVersion(string semver)
        {
            return new SemanticVersion(semver);
        }

        public static implicit operator string(SemanticVersion semver)
        {
            if (semver == null)
                throw new ArgumentNullException("semver");

            return semver.ToString();
        }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(Label))
                return String.Format("{0}.{1}.{2}", Major, Minor, Patch);

            return String.Format("{0}.{1}.{2}-{3}", Major, Minor, Patch, Label);
        }

        private void ParseFromString(string semver)
        {
            if (String.IsNullOrEmpty(semver))
                throw new ArgumentNullException("semver");

            var label = String.Empty;

            if (semver.Contains("-"))
            {
                label = semver.Substring(semver.IndexOf("-", StringComparison.Ordinal) + 1);
                semver = semver.Substring(0, semver.IndexOf("-", StringComparison.Ordinal));
            }

            var parts = semver.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 1 || parts.Length > 3)
                throw new ArgumentException("Invalid semantic version.", "semver");

            if (parts[0].Contains("-"))
                throw new ArgumentException("Invalid semantic version", "semver");

            Major = Convert.ToInt32(parts[0]);

            if (parts.Length > 1)
                Minor = Convert.ToInt32(parts[1]);

            if (parts.Length > 2)
                Patch = Convert.ToInt32(parts[2]);

            Label = label;
        }

        private static void ThrowIfNegative(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("value", "Value cannot be a negative integer.");
        }
    }
}
