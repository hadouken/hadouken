### New in 1.3.4 (Released 2013-05-07)
    FIX: The HTTP server did not load the UI.
    FIX: The default key/value store now creates keys with default values (if specified).

### New in 1.3.5 (Released 2013-05-07)
    WORKAROUND: Disabled the GitHub API check when releasing a new version.

### New in 1.3.6 (Released 2013-05-26)
    FIX: Installer checks for .NET4 (full) when installing.
    FIX: Installer puts values in registry as strings to avoid '#'-padding.

### New in 1.4.1 (Released 2013-06-11)
    FIX: Made Hadouken dependant on .NET 4.5 to avoid mismatch between developer machines and build servers.
    FIX: Installer now cannot install as a service.

### New in 1.4.2 (Released 2013-06-25)
    FIX: The default for Hadouken is now to listen on all available addresses.
    FIX: A bug when using the '+' character in the binding has been resolved.