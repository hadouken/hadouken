using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Hadouken.Common.IO {
    /// <summary>
    ///     Compares <see cref="Path" /> instances.
    /// </summary>
    public sealed class PathComparer : IEqualityComparer<Path> {
        /// <summary>
        ///     The default path comparer.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly PathComparer Default = new PathComparer(Machine.IsUnix());

        private readonly bool _isCaseSensitive;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PathComparer" /> class.
        /// </summary>
        /// <param name="isCaseSensitive">if set to <c>true</c>, comparison is case sensitive.</param>
        public PathComparer(bool isCaseSensitive) {
            this._isCaseSensitive = isCaseSensitive;
        }

        /// <summary>
        ///     Gets a value indicating whether comparison is case sensitive.
        /// </summary>
        /// <value>
        ///     <c>true</c> if comparison is case sensitive; otherwise, <c>false</c>.
        /// </value>
        public bool IsCaseSensitive {
            get { return this._isCaseSensitive; }
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Path" /> instances are equal.
        /// </summary>
        /// <param name="x">The first <see cref="Path" /> to compare.</param>
        /// <param name="y">The second <see cref="Path" /> to compare.</param>
        /// <returns>
        ///     True if the specified <see cref="Path" /> instances are equal; otherwise, false.
        /// </returns>
        public bool Equals(Path x, Path y) {
            if (x == null && y == null) {
                return true;
            }
            if (x == null || y == null) {
                return false;
            }

            if (this.IsCaseSensitive) {
                return x.FullPath.Equals(y.FullPath);
            }
            return x.FullPath.Equals(y.FullPath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Returns a hash code for the specified <see cref="Path" />.
        /// </summary>
        /// <param name="obj">The path.</param>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(Path obj) {
            if (obj == null) {
                throw new ArgumentNullException("obj");
            }
            if (this.IsCaseSensitive) {
                return obj.FullPath.GetHashCode();
            }
            return obj.FullPath.ToUpperInvariant().GetHashCode();
        }
    }
}