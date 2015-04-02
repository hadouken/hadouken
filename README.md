# Overview

[![Gitter chat](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/hadouken/hadouken)

[![Hadouken documentation](https://readthedocs.org/projects/hadouken/badge/?version=latest)](http://docs.hdkn.net)

**Build status**

 * Windows: [![Build Status](https://builds.nullreferenceexception.se/app/rest/builds/buildType:id:hadouken_core_ContinuousWindows/statusIcon)](https://builds.nullreferenceexception.se/viewType.html?buildTypeId=hadouken_core_ContinuousWindows&guest=1)
 * Ubuntu: [![Build Status](https://builds.nullreferenceexception.se/app/rest/builds/buildType:id:hadouken_core_ContinuousUbuntu/statusIcon)](https://builds.nullreferenceexception.se/viewType.html?buildTypeId=hadouken_core_ContinuousUbuntu&guest=1)

Hadouken is a modern, open source, cross-platform BitTorrent client written in C++11. It is written using Poco and Rasterbar-libtorrent and designed to run headless as a native Linux daemon/Windows Service.

## Getting started

Please refer to [the documentation](http://docs.hdkn.net).

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
