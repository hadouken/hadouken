using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Plugins;
using Hadouken.Plugins.Metadata;
using NSubstitute;
using Xunit;

namespace Hadouken.Tests.Plugins
{
    public class PluginEngineTests
    {
        public class TheConstructor
        {
            [Fact]
            public void Should_Throw_ArgumentNullException_If_PluginScanners_Is_Null()
            {
                // Given, When, Then
                Assert.Throws<ArgumentNullException>(
                    () =>
                        new PluginEngine(null, Substitute.For<IPackageInstaller>(), Substitute.For<IPackageDownloader>()));
            }

            [Fact]
            public void Should_Throw_ArgumentNullException_If_PackageInstaller_Is_Null()
            {
                // Given, When, Then
                Assert.Throws<ArgumentNullException>(
                    () =>
                        new PluginEngine(new[] { Substitute.For<IPluginScanner>() }, null, Substitute.For<IPackageDownloader>()));
            }

            [Fact]
            public void Should_Throw_ArgumentNullException_If_PackageDownloader_Is_Null()
            {
                // Given, When, Then
                Assert.Throws<ArgumentNullException>(
                    () =>
                        new PluginEngine(new[] { Substitute.For<IPluginScanner>() }, Substitute.For<IPackageInstaller>(), null));
            }
        }

        public class TheScanMethod
        {
            [Fact]
            public void Should_Not_Add_The_Same_Plugin_When_Scanning_Multiple_Times()
            {
                // Given
                var scanner = Substitute.For<IPluginScanner>();
                scanner.Scan().Returns(new[] {Substitute.For<IPluginManager>()});
                var engine = new PluginEngine(
                    new[] {scanner},
                    Substitute.For<IPackageInstaller>(),
                    Substitute.For<IPackageDownloader>());

                // When
                engine.Scan();
                engine.Scan(); // Scan a second time

                // Then
                Assert.Equal(1, engine.GetAll().Count());
            }

            [Fact]
            public void Should_Add_New_Plugins_When_Called_A_Second_Time()
            {
                // Given
                var scanner = Substitute.For<IPluginScanner>();
                var engine = new PluginEngine(
                    new[] { scanner },
                    Substitute.For<IPackageInstaller>(),
                    Substitute.For<IPackageDownloader>());

                // When, Then
                engine.Scan();
                Assert.Equal(0, engine.GetAll().Count());


                // When
                scanner.Scan().Returns(new[] { Substitute.For<IPluginManager>() });
                engine.Scan(); // Scan a second time

                // Then
                Assert.Equal(1, engine.GetAll().Count());
            }
        }

        public class TheLoadMethod
        {
            [Fact]
            public void Should_Not_Load_Plugin_That_Has_Missing_Dependency()
            {
                // Given
                var manager = Substitute.For<IPluginManager>();
                manager.Manifest.Name.Returns("a");
                manager.Manifest.Dependencies.Returns(new[] {new Dependency {Name = "missing"}});
                var scanner = Substitute.For<IPluginScanner>();
                scanner.Scan().Returns(new[] {manager});
                var downloader = Substitute.For<IPackageDownloader>();
                downloader.Download(Arg.Any<string>()).Returns(c => null);
                var engine = new PluginEngine(
                    new[] { scanner },
                    Substitute.For<IPackageInstaller>(),
                    downloader);

                // When
                engine.Scan();
                engine.Load("a");

                // Then
                manager.DidNotReceive().Load();
            }

            [Fact]
            public void Should_Load_Plugin_Having_No_Dependencies()
            {
                // Given
                var manager = Substitute.For<IPluginManager>();
                manager.Manifest.Name.Returns("a");
                var scanner = Substitute.For<IPluginScanner>();
                scanner.Scan().Returns(new[] { manager });
                var engine = new PluginEngine(
                    new[] { scanner },
                    Substitute.For<IPackageInstaller>(),
                    Substitute.For<IPackageDownloader>());

                // When
                engine.Scan();
                engine.Load("a");

                // Then
                manager.Received(1).Load();
            }

            [Fact]
            public void Should_Load_Plugin_And_All_Its_Dependencies()
            {
                // Given
                var m1 = CreatePluginManager("a");
                var m2 = CreatePluginManager("b", "a");
                var m3 = CreatePluginManager("c", "b", "a");
                var m4 = CreatePluginManager("d", "c");
                var scanner = Substitute.For<IPluginScanner>();
                scanner.Scan().Returns(new[] {m1, m2, m3, m4});
                var engine = new PluginEngine(new[]{scanner},
                    Substitute.For<IPackageInstaller>(),
                    Substitute.For<IPackageDownloader>());

                // When
                engine.Scan();
                engine.Load("d");

                // Then
                m1.Received().Load();
                m2.Received().Load();
                m3.Received().Load();
                m4.Received().Load();
            }

            [Fact]
            public void Should_Not_Load_Any_Plugin_If_Chain_Has_Missing_Dependency()
            {
                // Given
                var m1 = CreatePluginManager("a", "x");
                var m2 = CreatePluginManager("b", "a");
                var m3 = CreatePluginManager("c", "b", "a");
                var m4 = CreatePluginManager("d", "c");
                var scanner = Substitute.For<IPluginScanner>();
                scanner.Scan().Returns(new[] { m1, m2, m3, m4 });
                var downloader = Substitute.For<IPackageDownloader>();
                downloader.Download(Arg.Any<string>()).Returns(_ => null);
                var engine = new PluginEngine(new[] { scanner },
                    Substitute.For<IPackageInstaller>(),
                    downloader);

                // When
                engine.Scan();
                engine.Load("d");

                // Then
                downloader.Received().Download("x");
                m1.DidNotReceive().Load();
                m2.DidNotReceive().Load();
                m3.DidNotReceive().Load();
                m4.DidNotReceive().Load();
            }

            [Fact]
            public void Should_Download_And_Install_Missing_Dependency()
            {
                // Given
                var list = new List<IPluginManager>();
                var m = CreatePluginManager("a", "x");
                list.Add(m);

                var scanner = Substitute.For<IPluginScanner>();
                scanner.Scan().Returns(list);
                var installer = Substitute.For<IPackageInstaller>();
                installer.When(i => i.Install(Arg.Any<IPackage>())).Do(_ => list.Add(CreatePluginManager("x")));
                var downloader = Substitute.For<IPackageDownloader>();
                var pkg = Substitute.For<IPackage>();
                pkg.Manifest.Name.Returns("x");
                downloader.Download("x").Returns(pkg);
                var engine = new PluginEngine(new[] { scanner }, installer, downloader);

                // When
                engine.Scan();
                engine.Load("a");

                // Then
                downloader.Received().Download("x");
                installer.Received().Install(Arg.Any<IPackage>());
                m.Received().Load();
            }

            [Fact]
            public void Should_Not_Load_Any_Plugin_If_Downloaded_Dependency_Has_Missing_Dependency()
            {
                // Given
                var list = new List<IPluginManager>();
                var m = CreatePluginManager("a", "x");
                list.Add(m);

                var scanner = Substitute.For<IPluginScanner>();
                scanner.Scan().Returns(list);
                var installer = Substitute.For<IPackageInstaller>();
                installer.When(i => i.Install(Arg.Any<IPackage>())).Do(_ => list.Add(CreatePluginManager("x", "y")));
                var downloader = Substitute.For<IPackageDownloader>();
                var pkg = Substitute.For<IPackage>();
                pkg.Manifest.Name.Returns("x");
                downloader.Download("x").Returns(pkg);
                downloader.Download("y").Returns(_ => null);
                var engine = new PluginEngine(new[] { scanner }, installer, downloader);

                // When
                engine.Scan();
                engine.Load("a");

                // Then
                downloader.Received().Download("x");
                downloader.Received().Download("y");
                installer.Received(1).Install(Arg.Any<IPackage>());
                m.DidNotReceive().Load();
            }

            private static IPluginManager CreatePluginManager(string name, params string[] dependencies)
            {
                var m = Substitute.For<IPluginManager>();
                m.Manifest.Name.Returns(name);

                var deps = new List<Dependency>();

                if (dependencies != null && dependencies.Any())
                {
                    deps.AddRange(dependencies.Select(dependency => new Dependency {Name = dependency}));
                    m.Manifest.Dependencies.Returns(deps.ToArray());
                }

                return m;
            }
        }
    }
}
