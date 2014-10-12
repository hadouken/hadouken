﻿using System.IO;

namespace Hadouken.Common.IO
{
    /// <summary>
    /// Represents a file.
    /// </summary>
    public interface IFile
    {
        /// <summary>
        /// Gets the path to the file.
        /// </summary>
        /// <value>The path.</value>
        FilePath Path { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IFile"/> exists.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the file exists; otherwise, <c>false</c>.
        /// </value>
        bool Exists { get; }

        /// <summary>
        /// Gets the length of the file.
        /// </summary>
        /// <value>The length of the file.</value>
        long Length { get; }

        /// <summary>
        /// Copies the file to the specified destination path.
        /// </summary>
        /// <param name="destination">The destination path.</param>
        /// <param name="overwrite">Will overwrite existing destination file if set to <c>true</c>.</param>
        void Copy(FilePath destination, bool overwrite);

        /// <summary>
        /// Moves the file to the specified destination path.
        /// </summary>
        /// <param name="destination">The destination path.</param>
        void Move(FilePath destination);

        /// <summary>
        /// Deletes the file.
        /// </summary>
        void Delete();

        /// <summary>
        /// Opens the file using the specified options.
        /// </summary>
        /// <param name="fileMode">The file mode.</param>
        /// <param name="fileAccess">The file access.</param>
        /// <param name="fileShare">The file share.</param>
        /// <returns>A <see cref="Stream"/> to the file.</returns>
        Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare);        
    }
}