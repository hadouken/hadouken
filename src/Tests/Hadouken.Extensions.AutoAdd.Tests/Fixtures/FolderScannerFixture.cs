using Hadouken.Common.Data;
using Hadouken.Common.IO;
using Hadouken.Common.Messaging;
using Hadouken.Extensions.AutoAdd.Data;
using NSubstitute;

namespace Hadouken.Extensions.AutoAdd.Tests.Fixtures {
    internal sealed class FolderScannerFixture {
        public FolderScannerFixture() {
            this.FileSystem = Substitute.For<IFileSystem>();
            this.KeyValueStore = Substitute.For<IKeyValueStore>();
            this.AutoAddRepository = Substitute.For<IAutoAddRepository>();
            this.MessageBus = Substitute.For<IMessageBus>();
        }

        public IFileSystem FileSystem { get; set; }
        public IKeyValueStore KeyValueStore { get; set; }
        public IAutoAddRepository AutoAddRepository { get; set; }
        public IMessageBus MessageBus { get; set; }

        public FolderScanner CreateScanner() {
            return new FolderScanner(this.FileSystem, this.KeyValueStore, this.AutoAddRepository, this.MessageBus);
        }
    }
}