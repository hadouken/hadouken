///////////////////////////////////////////////////////////////////////
// Portions of this code was ported from glob-js by Kevin Thompson.
// https://github.com/kthompson/glob-js
///////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Hadouken.Common.IO.Globbing.Nodes;
using Hadouken.Common.IO.Globbing.Nodes.Roots;

namespace Hadouken.Common.IO.Globbing {
    internal sealed class Parser {
        private readonly IEnvironment _environment;
        private readonly Scanner _scanner;
        private Token _currentToken;

        public Parser(Scanner scanner, IEnvironment environment) {
            this._scanner = scanner;
            this._environment = environment;
            this._currentToken = null;
        }

        public List<Node> Parse() {
            this.Accept();

            // Parse the root.
            var items = new List<Node> {this.ParseRoot()};
            if (items.Count == 1 && items[0] is RelativeRoot) {
                items.Add(this.ParseSegment());
            }

            // Parse all path segments.
            while (this._currentToken.Kind == TokenKind.PathSeparator) {
                this.Accept();
                items.Add(this.ParseSegment());
            }

            // Not an end of text token?
            if (this._currentToken.Kind != TokenKind.EndOfText) {
                throw new InvalidOperationException("Expected EOT");
            }

            // Return the path node.
            return items;
        }

        private RootNode ParseRoot() {
            if (this._environment.IsUnix()) {
                // Starts with a separator?
                if (this._currentToken.Kind == TokenKind.PathSeparator) {
                    return new UnixRoot();
                }
            }
            else {
                // Starts with a separator?
                if (this._currentToken.Kind == TokenKind.PathSeparator) {
                    if (this._scanner.Peek().Kind == TokenKind.PathSeparator) {
                        throw new NotSupportedException("UNC paths are not supported.");
                    }
                    return new WindowsRoot(string.Empty);
                }

                // Is this a drive?
                if (this._currentToken.Kind == TokenKind.Identifier && this._currentToken.Value.Length == 1 &&
                    this._scanner.Peek().Kind == TokenKind.WindowsRoot) {
                    var identifier = this.ParseIdentifier();
                    this.Accept(TokenKind.WindowsRoot);
                    return new WindowsRoot(identifier.Identifier);
                }
            }

            // Starts with an identifier?
            if (this._currentToken.Kind != TokenKind.Identifier) {
                throw new NotImplementedException();
            }
            // Is the identifer indicating a current directory?
            if (this._currentToken.Value != ".") {
                return new RelativeRoot();
            }
            this.Accept();
            if (this._currentToken.Kind != TokenKind.PathSeparator) {
                throw new InvalidOperationException();
            }
            this.Accept();
            return new RelativeRoot();
        }

        private Node ParseSegment() {
            if (this._currentToken.Kind == TokenKind.DirectoryWildcard) {
                this.Accept();
                return new WildcardSegmentNode();
            }

            var items = new List<Node>();
            while (true) {
                switch (this._currentToken.Kind) {
                    case TokenKind.Identifier:
                    case TokenKind.CharacterWildcard:
                    case TokenKind.Wildcard:
                        items.Add(this.ParseSubSegment());
                        continue;
                }
                break;
            }
            return new SegmentNode(items);
        }

        private Node ParseSubSegment() {
            switch (this._currentToken.Kind) {
                case TokenKind.Identifier:
                    return this.ParseIdentifier();
                case TokenKind.CharacterWildcard:
                case TokenKind.Wildcard:
                    return this.ParseWildcard(this._currentToken.Kind);
            }

            throw new NotSupportedException("Unable to parse sub segment.");
        }

        private IdentifierNode ParseIdentifier() {
            if (this._currentToken.Kind != TokenKind.Identifier) {
                throw new InvalidOperationException("Unable to parse identifier.");
            }
            var identifier = new IdentifierNode(this._currentToken.Value);
            this.Accept();
            return identifier;
        }

        private Node ParseWildcard(TokenKind kind) {
            this.Accept(kind);
            return new WildcardNode(kind);
        }

        private void Accept(TokenKind kind) {
            if (this._currentToken.Kind != kind) {
                throw new InvalidOperationException("Unexpected token kind.");
            }
            this.Accept();
        }

        private void Accept() {
            this._currentToken = this._scanner.Scan();
        }
    }
}