using System.IO;

namespace Hadouken.Common.Reflection {
    public interface IEmbeddedResource {
        string FullName { get; }
        string Name { get; }
        Stream OpenRead();
    }
}