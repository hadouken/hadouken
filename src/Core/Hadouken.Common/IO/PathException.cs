using System;

namespace Hadouken.Common.IO {
    [Serializable]
    public class PathException : Exception {
        public PathException(string message)
            : base(message) {}
    }
}