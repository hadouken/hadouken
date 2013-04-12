Hadouken BitTorrent client
==========================

Hadouken is a BitTorrent client written in C# and designed to run as a Windows Service. It has plugin support and is managed completely through a web UI.

All development is done in the `develop` branch using the Git Flow extension (with default settings).

Building from source
--------------------
You need Ruby installed to build from source. Install Ruby, then run `gem install bundler`, `bundle install` and lastly `rake` to build. This will build zip and MSI packages for both x86 and x64 architectures.