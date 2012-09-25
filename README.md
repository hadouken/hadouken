Hadouken BitTorrent client
==========================

Hadouken is a BitTorrent client written in C# and designed to run as a Windows Service. It has plugin support and is managed completely through a web UI.

Building from source
--------------------
You need Ruby installed to build from source. Install Ruby, then run `gem install bundler`, `bundle install` and lastly `rake` to build. This will build an alpha/x86 build with release configuration. To build x64 builds, type `rake arch:x64 alpha`.