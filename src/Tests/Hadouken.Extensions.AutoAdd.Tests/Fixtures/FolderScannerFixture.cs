using Hadouken.Common.Data;
using Hadouken.Common.IO;
using Hadouken.Common.Messaging;
using Hadouken.Extensions.AutoAdd.Data;
using NSubstitute;

namespace Hadouken.Extensions.AutoAdd.Tests.Fixtures
{
    internal sealed class FolderScannerFixture
    {
        public FolderScannerFixture()
        {
            FileSystem = Substitute.For<IFileSystem>();
            KeyValueStore = Substitute.For<IKeyValueStore>();
            AutoAddRepository = Substitute.For<IAutoAddRepository>();
            MessageBus = Substitute.For<IMessageBus>();
        }

        public IFileSystem FileSystem { get; set; }

        public IKeyValueStore KeyValueStore { get; set; }

        public IAutoAddRepository AutoAddRepository { get; set; }

        public IMessageBus MessageBus { get; set; }

        public FolderScanner CreateScanner()
        {
            return new FolderScanner(FileSystem, KeyValueStore, AutoAddRepository, MessageBus);
        }
    }
}
