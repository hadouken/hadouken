﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hadouken.Common.IO {
    /// <summary>
    ///     A collection of <see cref="DirectoryPath" />.
    /// </summary>
    public sealed class DirectoryPathCollection : IEnumerable<DirectoryPath> {
        private readonly PathComparer _comparer;
        private readonly HashSet<DirectoryPath> _paths;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DirectoryPathCollection" /> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public DirectoryPathCollection(PathComparer comparer)
            : this(Enumerable.Empty<DirectoryPath>(), comparer) {}

        /// <summary>
        ///     Initializes a new instance of the <see cref="DirectoryPathCollection" /> class.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="System.ArgumentNullException">comparer</exception>
        public DirectoryPathCollection(IEnumerable<DirectoryPath> paths, PathComparer comparer) {
            if (comparer == null) {
                throw new ArgumentNullException("comparer");
            }
            this._comparer = comparer;
            this._paths = new HashSet<DirectoryPath>(paths, comparer);
        }

        /// <summary>
        ///     Gets the number of directories in the collection.
        /// </summary>
        /// <value>The number of directories in the collection.</value>
        public int Count {
            get { return this._paths.Count; }
        }

        internal PathComparer Comparer {
            get { return this._comparer; }
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<DirectoryPath> GetEnumerator() {
            return this._paths.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        /// <summary>
        ///     Adds the specified path to the collection.
        /// </summary>
        /// <param name="path">The path to add.</param>
        /// <returns>
        ///     <c>true</c> if the path was added; <c>false</c> if the path was already present.
        /// </returns>
        public bool Add(DirectoryPath path) {
            return this._paths.Add(path);
        }

        /// <summary>
        ///     Adds the specified paths to the collection.
        /// </summary>
        /// <param name="paths">The paths to add.</param>
        public void Add(IEnumerable<DirectoryPath> paths) {
            foreach (var path in paths) {
                this._paths.Add(path);
            }
        }

        /// <summary>
        ///     Removes the specified path from the collection.
        /// </summary>
        /// <param name="path">The path to remove.</param>
        /// <returns>
        ///     <c>true</c> if the path was removed; <c>false</c> if the path was not found in the collection.
        /// </returns>
        public bool Remove(DirectoryPath path) {
            return this._paths.Remove(path);
        }

        /// <summary>
        ///     Removes the specified paths from the collection.
        /// </summary>
        /// <param name="paths">The paths to remove.</param>
        public void Remove(IEnumerable<DirectoryPath> paths) {
            foreach (var path in paths) {
                this._paths.Remove(path);
            }
        }

        /// <summary>Adds a path to the collection.</summary>
        /// <param name="collection">The collection.</param>
        /// <param name="path">The path to add.</param>
        /// <returns>
        ///     A new <see cref="DirectoryPathCollection" /> that contains the provided path as
        ///     well as the paths in the original collection.
        /// </returns>
        public static DirectoryPathCollection operator +(DirectoryPathCollection collection, DirectoryPath path) {
            var result = new DirectoryPathCollection(collection, collection.Comparer) {path};
            return result;
        }

        /// <summary>Adds multiple paths to the collection.</summary>
        /// <param name="collection">The collection.</param>
        /// <param name="paths">The paths to add.</param>
        /// <returns>A new <see cref="DirectoryPathCollection" /> with the content of both collections.</returns>
        public static DirectoryPathCollection operator +(
            DirectoryPathCollection collection, IEnumerable<DirectoryPath> paths) {
            var result = new DirectoryPathCollection(collection, collection.Comparer) {paths};
            return result;
        }

        /// <summary>
        ///     Removes a path from the collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="path">The path to remove.</param>
        /// <returns>A new <see cref="DirectoryPathCollection" /> that do not contain the provided path.</returns>
        public static DirectoryPathCollection operator -(DirectoryPathCollection collection, DirectoryPath path) {
            var result = new DirectoryPathCollection(collection, collection.Comparer);
            result.Remove(path);
            return result;
        }

        /// <summary>
        ///     Removes multiple paths from the collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="paths">The paths to remove.</param>
        /// <returns>A new <see cref="DirectoryPathCollection" /> that do not contain the provided paths.</returns>
        public static DirectoryPathCollection operator -(
            DirectoryPathCollection collection, IEnumerable<DirectoryPath> paths) {
            var result = new DirectoryPathCollection(collection, collection.Comparer);
            result.Remove(paths);
            return result;
        }
    }
}