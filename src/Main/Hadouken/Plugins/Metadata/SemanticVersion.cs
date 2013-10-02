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

        private static void ThrowIfNegative(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("value", "Value cannot be a negative integer.");
        }
    }
}
