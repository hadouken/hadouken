# Overview

 * Windows: [![Build Status](https://builds.nullreferenceexception.se/app/rest/builds/buildType:id:hadouken_core_ContinuousWindows/statusIcon)](https://builds.nullreferenceexception.se/viewType.html?buildTypeId=hadouken_core_ContinuousWindows&guest=1)
 * Ubuntu: [![Build Status](https://builds.nullreferenceexception.se/app/rest/builds/buildType:id:hadouken_core_ContinuousUbuntu/statusIcon)](https://builds.nullreferenceexception.se/viewType.html?buildTypeId=hadouken_core_ContinuousUbuntu&guest=1)

Hadouken is a modern, open source, cross-platform BitTorrent client written in C++11. It is written using Poco and Rasterbar-libtorrent and designed to run headless as a native Linux daemon/Windows Service.

## Building

The following will get you started building Hadouken for various platforms. This is currently the only way of getting binaries for different platforms and will be until development has stabilized.

*Hadouken uses CMake to generate makefiles for different platforms. Make sure CMake (>= 2.8) is installed and in your path.*

### Windows

Running the build script in a clean repository will install dependencies and build Hadouken in debug configuration.

```posh
PS> .\win32\build.ps1
```

### Linux

*TODO*
