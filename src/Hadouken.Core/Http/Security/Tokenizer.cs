using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Hadouken.Core.Http.Security.Storage;
using Nancy;
using Nancy.ErrorHandling;
using Nancy.Security;

namespace Hadouken.Core.Http.Security
{
    /// <summary>
    /// Default implementation of <see cref="ITokenizer"/>
    /// </summary>
    public class Tokenizer : ITokenizer
    {
        private readonly TokenValidator _validator;
        private readonly TokenKeyRing _keyRing;
        private ITokenKeyStore _keyStore = new InMemoryTokenKeyStore();
        private Encoding _encoding = Encoding.UTF8;
        private string _claimsDelimiter = "|";
        private string _hashDelimiter = ":";
        private string _itemDelimiter = Environment.NewLine;
        private Func<DateTime> _tokenStamp = () => DateTime.UtcNow;
        private Func<DateTime> _now = () => DateTime.UtcNow;
        private Func<TimeSpan> _tokenExpiration = () => TimeSpan.FromDays(1);
        private Func<TimeSpan> _keyExpiration = () => TimeSpan.FromDays(7);

        private Func<NancyContext, string>[] _additionalItems =
        {
            ctx => ctx.Request.Headers.UserAgent
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Tokenizer"/> class.
        /// </summary>
        public Tokenizer()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tokenizer"/> class.
        /// </summary>
        /// <param name="configuration">The configuration that should be used by the tokenizer.</param>
        public Tokenizer(Action<TokenizerConfigurator> configuration)
        {
            if (configuration != null)
            {
                var configurator = new TokenizerConfigurator(this);
                configuration.Invoke(configurator);
            }
            _keyRing = new TokenKeyRing(this);
            _validator = new TokenValidator(_keyRing);
        }

        /// <summary>
        /// Creates a token from a <see cref="IUserIdentity"/>.
        /// </summary>
        /// <param name="userIdentity">The user identity from which to create a token.</param>
        /// <param name="context">NancyContext</param>
        /// <returns>The generated token.</returns>
        public string Tokenize(IUserIdentity userIdentity, NancyContext context)
        {
            var items = new List<string>
            {
                userIdentity.UserName,
                string.Join(_claimsDelimiter, userIdentity.Claims),
                _tokenStamp().Ticks.ToString(CultureInfo.InvariantCulture)
            };

            foreach (var item in _additionalItems.Select(additionalItem => additionalItem(context)))
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    throw new RouteExecutionEarlyExitException(new Response { StatusCode = HttpStatusCode.Unauthorized });
                }

                items.Add(item);
            }

            var message = string.Join(_itemDelimiter, items);
            var token = CreateToken(message);
            return token;
        }

        /// <summary>
        /// Creates a <see cref="IUserIdentity"/> from a token.
        /// </summary>
        /// <param name="token">The token from which to create a user identity.</param>
        /// <param name="context">NancyContext</param>
        /// <returns>The detokenized user identity.</returns>
        public IUserIdentity Detokenize(string token, NancyContext context)
        {
            var tokenComponents = token.Split(new[] { _hashDelimiter }, StringSplitOptions.None);
            if (tokenComponents.Length != 2) return null;

            var messagebytes = Convert.FromBase64String(tokenComponents[0]);
            var hash = Convert.FromBase64String(tokenComponents[1]);

            if (!_validator.IsValid(messagebytes, hash))
            {
                return null;
            }

            var items = _encoding.GetString(messagebytes).Split(new[] { _itemDelimiter }, StringSplitOptions.None);
            var additionalItemCount = _additionalItems.Count();

            for (var i = 0; i < additionalItemCount; i++)
            {
                var tokenizedValue = items[i + 3];
                var currentValue = _additionalItems.ElementAt(i)(context);

                if (tokenizedValue != currentValue)
                {
                    // todo: may need to log here as this probably indicates hacking
                    return null;
                }
            }

            var generatedOn = new DateTime(long.Parse(items[2]));

            if (_tokenStamp() - generatedOn > _tokenExpiration())
            {
                return null;
            }

            var userName = items[0];
            var claims = items[1].Split(new[] { _claimsDelimiter }, StringSplitOptions.None);

            return new TokenUserIdentity(userName, claims);
        }

        private string CreateToken(string message)
        {
            var messagebytes = _encoding.GetBytes(message);
            var hash = _validator.CreateHash(messagebytes);
            return Convert.ToBase64String(messagebytes) + _hashDelimiter + Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Provides an API for configuring a <see cref="Tokenizer"/> instance.
        /// </summary>
        public class TokenizerConfigurator
        {
            private readonly Tokenizer _tokenizer;

            /// <summary>
            /// Initializes a new instance of the <see cref="TokenizerConfigurator"/> class.
            /// </summary>
            /// <param name="tokenizer"></param>
            public TokenizerConfigurator(Tokenizer tokenizer)
            {
                _tokenizer = tokenizer;
            }

            /// <summary>
            /// Sets the token key store ued by the tokenizer
            /// </summary>
            /// <param name="store"></param>
            /// <returns>A reference to the current <see cref="TokenizerConfigurator"/></returns>
            public TokenizerConfigurator WithKeyCache(ITokenKeyStore store)
            {
                _tokenizer._keyStore = store;
                return this;
            }

            /// <summary>
            /// Sets the encoding used by the tokenizer
            /// </summary>
            /// <param name="encoding"></param>
            /// <returns>A reference to the current <see cref="TokenizerConfigurator"/></returns>
            public TokenizerConfigurator Encoding(Encoding encoding)
            {
                _tokenizer._encoding = encoding;
                return this;
            }

            /// <summary>
            /// Sets the delimiter between document and its hash
            /// </summary>
            /// <param name="hashDelimiter"></param>
            /// <returns>A reference to the current <see cref="TokenizerConfigurator"/></returns>
            public TokenizerConfigurator HashDelimiter(string hashDelimiter)
            {
                _tokenizer._hashDelimiter = hashDelimiter;
                return this;
            }

            /// <summary>
            /// Sets the delimiter between each item to be tokenized
            /// </summary>
            /// <param name="itemDelimiter"></param>
            /// <returns>A reference to the current <see cref="TokenizerConfigurator"/></returns>
            public TokenizerConfigurator ItemDelimiter(string itemDelimiter)
            {
                _tokenizer._itemDelimiter = itemDelimiter;
                return this;
            }

            /// <summary>
            /// Sets the delimiter between each claim
            /// </summary>
            /// <param name="claimsDelimiter"></param>
            /// <returns>A reference to the current <see cref="TokenizerConfigurator"/></returns>
            public TokenizerConfigurator ClaimsDelimiter(string claimsDelimiter)
            {
                _tokenizer._claimsDelimiter = claimsDelimiter;
                return this;
            }

            /// <summary>
            /// Sets the token expiration interval. An expired token will cause a user to become unauthorized (logged out). 
            /// Suggested value is 1 day (which is also the default).
            /// </summary>
            /// <param name="expiration"></param>
            /// <returns>A reference to the current <see cref="TokenizerConfigurator"/></returns>
            public TokenizerConfigurator TokenExpiration(Func<TimeSpan> expiration)
            {
                _tokenizer._tokenExpiration = expiration;

                if (_tokenizer._tokenExpiration() >= _tokenizer._keyExpiration())
                {
                    throw new ArgumentException("Token expiration must be less than key expiration", "expiration");
                }

                return this;
            }

            /// <summary>
            /// Sets the key expiration interval. Must be longer than the <see cref="TokenizerConfigurator.TokenExpiration"/> value. 
            /// When keys expire, they are scheduled to purge once any tokens they have been used to generate have expired.
            /// Suggested range is 2 to 14 days. The default is 7 days.
            /// </summary>
            /// <param name="expiration"></param>
            /// <returns>A reference to the current <see cref="TokenizerConfigurator"/></returns>
            public TokenizerConfigurator KeyExpiration(Func<TimeSpan> expiration)
            {
                _tokenizer._keyExpiration = expiration;

                if (_tokenizer._tokenExpiration() >= _tokenizer._keyExpiration())
                {
                    throw new ArgumentException("Key expiration must be greater than token expiration", "expiration");
                }

                return this;
            }

            /// <summary>
            /// Sets the token-generated-at timestamp
            /// </summary>
            /// <param name="tokenStamp"></param>
            /// <returns>A reference to the current <see cref="TokenizerConfigurator"/></returns>
            public TokenizerConfigurator TokenStamp(Func<DateTime> tokenStamp)
            {
                _tokenizer._tokenStamp = tokenStamp;
                return this;
            }

            /// <summary>
            /// Sets the current date/time.
            /// </summary>
            /// <param name="now"></param>
            /// <returns>A reference to the current <see cref="TokenizerConfigurator"/></returns>
            public TokenizerConfigurator Now(Func<DateTime> now)
            {
                _tokenizer._now = now;
                return this;
            }

            /// <summary>
            /// Sets any additional items to be included when tokenizing. The default includes Request.Headers.UserAgent.
            /// </summary>
            /// <param name="additionalItems"></param>
            /// <returns>A reference to the current <see cref="TokenizerConfigurator"/></returns>
            public TokenizerConfigurator AdditionalItems(params Func<NancyContext, string>[] additionalItems)
            {
                _tokenizer._additionalItems = additionalItems;
                return this;
            }
        }

        private class TokenUserIdentity : IUserIdentity
        {
            public TokenUserIdentity(string userName, IEnumerable<string> claims)
            {
                UserName = userName;
                Claims = claims;
            }

            public string UserName { get; private set; }

            public IEnumerable<string> Claims { get; private set; }
        }

        private class TokenValidator
        {
            private readonly TokenKeyRing _keyRing;

            internal TokenValidator(TokenKeyRing keyRing)
            {
                _keyRing = keyRing;
            }

            public bool IsValid(byte[] message, byte[] hash)
            {
                return _keyRing
                           .AllKeys()
                           .Select(key => GenerateHash(key, message))
                           .Any(hash.SequenceEqual);
            }

            public byte[] CreateHash(byte[] message)
            {
                var key = _keyRing
                              .NonExpiredKeys()
                              .First();

                return GenerateHash(key, message);
            }

            private static byte[] GenerateHash(byte[] key, byte[] message)
            {
                using (var hmac = new HMACSHA256(key))
                {
                    return hmac.ComputeHash(message);
                }
            }
        }

        private class TokenKeyRing
        {
            private readonly Tokenizer _tokenizer;
            private readonly IDictionary<DateTime, byte[]> _keys;

            internal TokenKeyRing(Tokenizer tokenizer)
            {
                _tokenizer = tokenizer;
                _keys = _tokenizer._keyStore.Retrieve();
            }

            public IEnumerable<byte[]> AllKeys()
            {
                return Keys(true);
            }

            public IEnumerable<byte[]> NonExpiredKeys()
            {
                return Keys(false);
            }

            private IEnumerable<byte[]> Keys(bool includeExpired)
            {
                var entriesToPurge = new List<DateTime>();
                var validKeys = new List<byte[]>();

                foreach (var entry in _keys.OrderByDescending(x => x.Key))
                {
                    if (IsReadyToPurge(entry))
                    {
                        entriesToPurge.Add(entry.Key);
                    }
                    else if (!IsExpired(entry) || includeExpired)
                    {
                        validKeys.Add(entry.Value);
                    }
                }

                var shouldStore = false;

                foreach (var entry in entriesToPurge)
                {
                    _keys.Remove(entry);
                    shouldStore = true;
                }

                if (validKeys.Count == 0)
                {
                    var key = CreateKey();
                    _keys[_tokenizer._now()] = key;
                    validKeys.Add(key);
                    shouldStore = true;
                }

                if (shouldStore)
                {
                    _tokenizer._keyStore.Store(_keys);
                }

                return validKeys;
            }

            private bool IsReadyToPurge(KeyValuePair<DateTime, byte[]> entry)
            {
                return _tokenizer._now() - entry.Key > (_tokenizer._keyExpiration() + _tokenizer._tokenExpiration());
            }

            private bool IsExpired(KeyValuePair<DateTime, byte[]> entry)
            {
                return _tokenizer._now() - entry.Key > _tokenizer._keyExpiration();
            }

            private static byte[] CreateKey()
            {
                var secretKey = new byte[64];

                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(secretKey);
                }

                return secretKey;
            }
        }
    }
}
