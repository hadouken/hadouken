Hadouken BitTorrent client
==========================

Hadouken is a BitTorrent client written in C# and designed to run as a Windows Service. It has plugin support and is managed completely through a web UI.

Building from source
--------------------
After cloning the repository, run `Build.bat` to build a local copy of Hadouken. This will generate a `src/Shared/CommonAssemblyInfo.cs` needed for the Visual Studio projects, as well as creating a zip and MSI file in the `bin` folder.