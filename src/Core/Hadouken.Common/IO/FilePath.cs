using System;

namespace Hadouken.Common.IO {
    /// <summary>
    ///     Represents a file path.
    /// </summary>
    public sealed class FilePath : Path {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FilePath" /> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public FilePath(string path)
            : base(path) {}

        /// <summary>
        ///     Gets a value indicating whether this path has a file extension.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this file path has a file extension; otherwise, <c>false</c>.
        /// </value>
        public bool HasExtension {
            get { return System.IO.Path.HasExtension(this.FullPath); }
        }

        /// <summary>
        ///     Gets the directory part of the path.
        /// </summary>
        /// <returns>The directory part of the path.</returns>
        public DirectoryPath GetDirectory() {
            var directory = System.IO.Path.GetDirectoryName(this.FullPath);
            if (string.IsNullOrWhiteSpace(directory)) {
                directory = "./";
            }
            return new DirectoryPath(directory);
        }

        /// <summary>
        ///     Gets the filename.
        /// </summary>
        /// <returns>The filename.</returns>
        public FilePath GetFilename() {
            var filename = System.IO.Path.GetFileName(this.FullPath);
            return new FilePath(filename);
        }

        /// <summary>
        ///     Gets the file extension.
        /// </summary>
        /// <returns>Gets the file extension.</returns>
        public string GetExtension() {
            var extension = System.IO.Path.GetExtension(this.FullPath);
            return string.IsNullOrWhiteSpace(extension) ? null : extension;
        }

        /// <summary>
        ///     Changes the file extension of the path.
        /// </summary>
        /// <param name="extension">The new extension.</param>
        /// <returns>A new <see cref="FilePath" /> with a new extension.</returns>
        public FilePath ChangeExtension(string extension) {
            return new FilePath(System.IO.Path.ChangeExtension(this.FullPath, extension));
        }

        /// <summary>
        ///     Appends a file extension to the path.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>A new <see cref="FilePath" /> with an appended extension.</returns>
        public FilePath AppendExtension(string extension) {
            if (!extension.StartsWith(".", StringComparison.OrdinalIgnoreCase)) {
                extension = string.Concat(".", extension);
            }
            return new FilePath(string.Concat(this.FullPath, extension));
        }

        /// <summary>
        ///     Makes the path absolute (if relative) using the current working directory.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <returns>An absolute path.</returns>
        public FilePath MakeAbsolute(IEnvironment environment) {
            return this.IsRelative
                ? environment.GetApplicationRoot().CombineWithFilePath(this)
                : new FilePath(this.FullPath);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.String" /> to <see cref="FilePath" />.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A <see cref="FilePath" />.</returns>
        public static implicit operator FilePath(string path) {
            return FromString(path);
        }

        /// <summary>
        ///     Performs a conversion from <see cref="System.String" /> to <see cref="FilePath" />.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A <see cref="FilePath" />.</returns>
        public static FilePath FromString(string path) {
            return new FilePath(path);
        }
    }
}