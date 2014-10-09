using Hadouken.Common.IO;
using Hadouken.Common.Tests.Fakes;
using NSubstitute;

namespace Hadouken.Common.Tests.Fixtures
{
    public sealed class GlobberFixture
    {
        public IFileSystem FileSystem { get; set; }
        public IEnvironment Environment { get; set; }

        public GlobberFixture(bool isUnix = true)
        {
            FileSystem = CreateFileSystem(isUnix);
            Environment = CreateEnvironment(isUnix);
        }

        private static FakeFileSystem CreateFileSystem(bool isUnix)
        {
            var fileSystem = new FakeFileSystem(isUnix);
            fileSystem.GetCreatedDirectory("/Temp");
            fileSystem.GetCreatedDirectory("/Temp/Hello");
            fileSystem.GetCreatedDirectory("/Temp/Hello/World");
            fileSystem.GetCreatedDirectory("/Temp/Goodbye");
            fileSystem.GetCreatedFile("/Presentation.ppt");
            fileSystem.GetCreatedFile("/Budget.xlsx");
            fileSystem.GetCreatedFile("/Text.txt");
            fileSystem.GetCreatedFile("/Temp");
            fileSystem.GetCreatedFile("/Temp/Hello/World/Text.txt");
            fileSystem.GetCreatedFile("/Temp/Hello/World/Picture.png");
            fileSystem.GetCreatedFile("/Temp/Goodbye/OtherText.txt");
            fileSystem.GetCreatedFile("/Temp/Goodbye/OtherPicture.png");
            return fileSystem;
        }

        private static IEnvironment CreateEnvironment(bool isUnix)
        {
            var environment = Substitute.For<IEnvironment>();
            environment.IsUnix().Returns(isUnix);
            environment.GetApplicationRoot().Returns("/Working");
            return environment;
        }
    }
}
