# Overview

Hadouken is a modern, open source, cross-platform BitTorrent client written in C++11. It is written using Boost, Rasterbar-libtorrent and Google's V8 engine to make both the core and plugin ecosystem platform independent.

It is designed to run headless as a native Linux daemon/Windows Service.

Plugins has always been Hadoukens strong point, and the rewritten plugin framework is utilizing Google's V8 engine to expose a powerful plugin engine for JavaScript plugins.

## Building

The following will get you started building Hadouken for various platforms. This is currently the only way of getting binaries for different platforms and will be until development has stabilized.

*Hadouken uses CMake to generate makefiles for different platforms. Make sure CMake (>= 2.8) is installed and in your path.*

### Windows

Building Hadouken on Windows is done using a simple PowerShell script. The third-party components needed, ie. Boost, Rasterbar-libtorrent and OpenSSL, will be downloaded and installed from the NuGet packages `hadouken.boost`, `hadouken.libtorrent` and `hadouken.openssl`. This is to ensure a smooth setup for new developers.

After cloning the repository, open a PowerShell prompt in the root directory and execute `.\win32\prepare.ps1`. This will download and install the NuGet packages needed.

Next, run the following,

```posh
PS> mkdir build
PS> cd build
PS> cmake -G "Visual Studio 12" ..
PS> msbuild .\hadouken.sln
```

This will generate a Visual Studio solution with related projects as well as building Hadouken.

### Linux

For now, refer to the `.travis.yml` file for how to build on Linux.
