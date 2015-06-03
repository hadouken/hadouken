using System;
using System.Linq;

namespace Hadouken.Common.IO {
    /// <summary>
    ///     Provides properties and instance methods for working with paths.
    ///     This class must be inherited.
    /// </summary>
    public abstract class Path {
        private static readonly char[] InvalidPathCharacters;
        private readonly bool _isRelative;
        private readonly string _path;
        private readonly string[] _segments;

        static Path() {
            InvalidPathCharacters = System.IO.Path.GetInvalidPathChars().Concat(new[] {'*', '?'}).ToArray();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Path" /> class.
        /// </summary>
        /// <param name="path">The path.</param>
        protected Path(string path) {
            if (path == null) {
                throw new ArgumentNullException("path");
            }
            if (string.IsNullOrWhiteSpace(path)) {
                throw new ArgumentException("Path cannot be empty.", "path");
            }

            this._path = path.Replace('\\', '/').Trim();
            this._path = this._path == "./" ? string.Empty : this._path;

            // Remove relative part of a path.
            if (this._path.StartsWith("./", StringComparison.Ordinal)) {
                this._path = this._path.Substring(2);
            }

            // Remove trailing slashes.
            this._path = this._path.TrimEnd('/', '\\');

            if (this._path.EndsWith(":", StringComparison.OrdinalIgnoreCase)) {
                this._path = string.Concat(this._path, "/");
            }

            // Relative path?
            this._isRelative = !System.IO.Path.IsPathRooted(this._path);

            // Extract path segments.
            this._segments = this._path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);

            // Validate the path.
            foreach (var character in path.Where(character => InvalidPathCharacters.Contains(character))) {
                const string format = "Illegal characters in directory path ({0}).";
                throw new ArgumentException(string.Format(format, character), "path");
            }
        }

        /// <summary>
        ///     Gets the full path.
        /// </summary>
        /// <value>The full path.</value>
        public string FullPath {
            get { return this._path; }
        }

        /// <summary>
        ///     Gets a value indicating whether this path is relative.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this path is relative; otherwise, <c>false</c>.
        /// </value>
        public bool IsRelative {
            get { return this._isRelative; }
        }

        /// <summary>
        ///     Gets the segments making up the path.
        /// </summary>
        /// <value>The segments making up the path.</value>
        public string[] Segments {
            get { return this._segments; }
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this path.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() {
            return this.FullPath;
        }
    }
}