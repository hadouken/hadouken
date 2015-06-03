///////////////////////////////////////////////////////////////////////
// Portions of this code was ported from glob-js by Kevin Thompson.
// https://github.com/kthompson/glob-js
///////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Hadouken.Common.IO.Globbing {
    internal sealed class Scanner {
        private readonly Regex _identifierRegex;
        private readonly string _pattern;
        private char _currentCharacter;
        private string _currentContent;
        private TokenKind _currentKind;
        private int _sourceIndex;

        public Scanner(string pattern) {
            if (pattern == null) {
                throw new ArgumentNullException("pattern");
            }
            this._pattern = pattern;
            this._sourceIndex = 0;
            this._currentContent = string.Empty;
            this._currentCharacter = this._pattern[this._sourceIndex];
            this._identifierRegex = new Regex("^[0-9a-zA-Z\\. _-]$", RegexOptions.Compiled);
        }

        public Token Scan() {
            this._currentContent = string.Empty;
            this._currentKind = this.ScanToken();

            return new Token(this._currentKind, this._currentContent);
        }

        public Token Peek() {
            var index = this._sourceIndex;
            var token = this.Scan();
            this._sourceIndex = index;
            this._currentCharacter = this._pattern[this._sourceIndex];
            return token;
        }

        private TokenKind ScanToken() {
            if (this.IsAlphaNumberic(this._currentCharacter)) {
                while (this.IsAlphaNumberic(this._currentCharacter)) {
                    this.TakeCharacter();
                }
                return TokenKind.Identifier;
            }

            switch (this._currentCharacter) {
                case '*':
                    this.TakeCharacter();
                    if (this._currentCharacter != '*') {
                        return TokenKind.Wildcard;
                    }
                    this.TakeCharacter();
                    return TokenKind.DirectoryWildcard;
                case '?':
                    this.TakeCharacter();
                    return TokenKind.CharacterWildcard;
                case '/':
                case '\\':
                    this.TakeCharacter();
                    return TokenKind.PathSeparator;
                case ':':
                    this.TakeCharacter();
                    return TokenKind.WindowsRoot;
                case '\0':
                    return TokenKind.EndOfText;
            }

            throw new NotSupportedException("Unknown token");
        }

        private bool IsAlphaNumberic(char character) {
            return this._identifierRegex.IsMatch(character.ToString(CultureInfo.InvariantCulture));
        }

        private void TakeCharacter() {
            if (this._currentCharacter == '\0') {
                return;
            }

            this._currentContent += this._currentCharacter;
            if (this._sourceIndex == this._pattern.Length - 1) {
                this._currentCharacter = '\0';
                return;
            }

            this._currentCharacter = this._pattern[++this._sourceIndex];
        }
    }
}