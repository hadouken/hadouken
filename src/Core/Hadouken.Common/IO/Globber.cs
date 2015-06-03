﻿///////////////////////////////////////////////////////////////////////
// Portions of this code was ported from glob-js by Kevin Thompson.
// https://github.com/kthompson/glob-js
///////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Hadouken.Common.IO.Globbing;
using Hadouken.Common.IO.Globbing.Nodes;
using Hadouken.Common.IO.Globbing.Nodes.Roots;

namespace Hadouken.Common.IO {
    /// <summary>
    ///     Responsible for file system globbing.
    /// </summary>
    public sealed class Globber : IGlobber {
        private readonly IEnvironment _environment;
        private readonly IFileSystem _fileSystem;
        private readonly RegexOptions _options;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Globber" /> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="environment">The environment.</param>
        public Globber(IFileSystem fileSystem, IEnvironment environment) {
            if (fileSystem == null) {
                throw new ArgumentNullException("fileSystem");
            }
            if (environment == null) {
                throw new ArgumentNullException("environment");
            }
            this._fileSystem = fileSystem;
            this._environment = environment;
            this._options = RegexOptions.Singleline;

            if (!this._environment.IsUnix()) {
                // On non unix systems, we should ignore case.
                this._options |= RegexOptions.IgnoreCase;
            }
        }

        /// <summary>
        ///     Returns <see cref="Path" /> instances matching the specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <returns>
        ///     <see cref="Path" /> instances matching the specified pattern.
        /// </returns>
        public IEnumerable<Path> Match(string pattern) {
            var scanner = new Scanner(pattern);
            var parser = new Parser(scanner, this._environment);
            var path = parser.Parse();

            var rootNodes = new List<Node>();
            while (path.Count > 0) {
                // Pop the first path item.
                var segment = path[0];
                path.RemoveAt(0);

                if (segment.IsWildcard) {
                    path.Insert(0, segment);
                    break;
                }
                rootNodes.Add(segment);
            }

            // Fix up the tree.
            var newRoot = this.FixRootNode(rootNodes);
            if (newRoot != null) {
                rootNodes[0] = newRoot;
            }

            // Ge the root.
            var rootDirectory = new DirectoryPath(string.Join("/", rootNodes.Select(x => x.Render())));

            // Nothing left in the path?
            if (path.Count == 0) {
                // We have an absolute path with no wild cards.
                return new Path[] {rootDirectory};
            }

            // Walk the root and return the unique results.
            var segments = new Stack<Node>(((IEnumerable<Node>) path).Reverse());
            var results = this.Walk(rootDirectory, segments);
            return new HashSet<Path>(results, new PathComparer(this._environment.IsUnix())).ToArray();
        }

        private Node FixRootNode(List<Node> rootNodes) {
            // Windows root?
            var windowsRoot = rootNodes[0] as WindowsRoot;
            if (windowsRoot != null) {
                // No drive?
                if (string.IsNullOrWhiteSpace(windowsRoot.Drive)) {
                    // Get the drive from the working directory.
                    var workingDirectory = this._environment.GetApplicationRoot();
                    var root = workingDirectory.FullPath.Split('/').First();
                    return new IdentifierNode(root);
                }
            }

            // Relative root?
            var relativeRoot = rootNodes[0] as RelativeRoot;
            if (relativeRoot != null) {
                // Get the drive from the working directory.
                var workingDirectory = this._environment.GetApplicationRoot();
                return new IdentifierNode(workingDirectory.FullPath);
            }

            return null;
        }

        private List<Path> Walk(DirectoryPath rootPath, Stack<Node> segments) {
            var results = new List<Path>();
            var segment = segments.Pop();

            var expression = new Regex("^" + segment.Render() + "$", this._options);
            var isDirectoryWildcard = false;

            if (segment is WildcardSegmentNode) {
                segments.Push(segment);
                isDirectoryWildcard = true;
            }

            // Get all files and folders.
            var root = this._fileSystem.GetDirectory(rootPath);
            if (!root.Exists) {
                return results;
            }

            foreach (var directory in root.GetDirectories("*", SearchScope.Current)) {
                var part = directory.Path.FullPath.Substring(root.Path.FullPath.Length + 1);
                var pathTest = expression.IsMatch(part);

                var subWalkCount = 0;

                if (isDirectoryWildcard) {
                    // Walk recursivly down the segment.
                    var nextSegments = new Stack<Node>(segments.Reverse());
                    var subwalkResult = this.Walk(directory.Path, nextSegments);
                    if (subwalkResult.Count > 0) {
                        results.AddRange(subwalkResult);
                    }

                    subWalkCount++;
                }

                // Check without directory wildcard.
                if (segments.Count > subWalkCount && (subWalkCount == 1 || pathTest)) {
                    // Walk the next segment in the list.
                    var nextSegments = new Stack<Node>(segments.Skip(subWalkCount).Reverse());
                    var subwalkResult = this.Walk(directory.Path, nextSegments);
                    if (subwalkResult.Count > 0) {
                        results.AddRange(subwalkResult);
                    }
                }

                // Got a match?
                if (pathTest && segments.Count == 0) {
                    results.Add(directory.Path);
                }
            }

            foreach (var file in root.GetFiles("*", SearchScope.Current)) {
                var part = file.Path.FullPath.Substring(root.Path.FullPath.Length + 1);
                var pathTest = expression.IsMatch(part);

                // Got a match?
                if (pathTest && segments.Count == 0) {
                    results.Add(file.Path);
                }
                else if (pathTest) {
                    /////////////////////////////////////////////////////////////B
                    // We got a match, but we still have segments left.
                    // Is the next part a directory wild card?
                    /////////////////////////////////////////////////////////////

                    var nextNode = segments.Peek();
                    if (!(nextNode is WildcardSegmentNode)) {
                        continue;
                    }
                    var nextSegments = new Stack<Node>(segments.Skip(1).Reverse());
                    var subwalkResult = this.Walk(root.Path, nextSegments);
                    if (subwalkResult.Count > 0) {
                        results.AddRange(subwalkResult);
                    }
                }
            }
            return results;
        }
    }
}