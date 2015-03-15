# Overview

 * Windows: [![Build Status](https://builds.nullreferenceexception.se/app/rest/builds/buildType:id:hadouken_core_ContinuousWindows/statusIcon)](https://builds.nullreferenceexception.se/viewType.html?buildTypeId=hadouken_core_ContinuousWindows&guest=1)
 * Ubuntu: [![Build Status](https://builds.nullreferenceexception.se/app/rest/builds/buildType:id:hadouken_core_ContinuousUbuntu/statusIcon)](https://builds.nullreferenceexception.se/viewType.html?buildTypeId=hadouken_core_ContinuousUbuntu&guest=1)

Hadouken is a modern, open source, cross-platform BitTorrent client written in C++11. It is written using Poco and Rasterbar-libtorrent and designed to run headless as a native Linux daemon/Windows Service.

## Getting started

*We are currently moving from C# to C++ and therefore only Windows beta builds are available. These can be found on [the build server](https://builds.nullreferenceexception.se/viewType.html?buildTypeId=hadouken_core_ContinuousWindows&guest=1).*

 * Download the latest MSI installer.
 * Install with your preferred settings.
 * Go to [remote.hdkn.net](http://remote.hdkn.net) and finish the configuration.

The source code for [remote.hdkn.net](http://remote.hdkn.net) is [available on GitHub](https://github.com/hadouken/remote) and anyone can host their own remote.

## For developers

If you're interested in contributing, we recommend reading through the [contribution guidelines](CONTRIBUTING.md).

For further documentation regarding plugins or the JSONRPC API, please refer to [the wiki](https://github.com/hadouken/hadouken/wiki).

### Building

Depending on your platform (Windows/Linux), Hadouken have different prerequisites. Common for all platforms is CMake, so make sure you have CMake (>= v2.8) installed and available in your path.

#### Windows

Building on Windows requires Visual Studio 2013. Dependencies will be pulled from NuGet at build time.

```posh
PS> .\win32\build.ps1
```

This will build and package Hadouken as well as output .zip and .msi files in the `.\win32\build\out\Release` folder.

#### Linux

Building on Linux (Ubuntu) requires `libssl-dev`, `libboost-system-dev`, and then you need to manually compile both Rasterbar-libtorrent (>= v1.0.3) and Poco (>= v1.6.0)

Next, run,

```bash
$ mkdir cmake-build && cd cmake-build
$ cmake .. && make
```
