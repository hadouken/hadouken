# This script will download the third-party libraries required to build
# Hadouken on Windows.

# Defines where to put the packages
$PACKAGES_DIRECTORY = Join-Path (Convert-Path .) "packages"

# Versions
$BOOST_VERSION = "0.1.5"
$LIBTORRENT_VERSION = "0.1.3"
$OPENSSL_VERSION = "0.1.3"

# Nuget configuration section
$NUGET_FILE         = "nuget.exe"
$NUGET_TOOL         = Join-Path $PACKAGES_DIRECTORY $NUGET_FILE
$NUGET_DOWNLOAD_URL = "https://nuget.org/$NUGET_FILE"

function Download-File {
    param (
        [string]$url,
        [string]$target
    )

    $webClient = new-object System.Net.WebClient
    $webClient.DownloadFile($url, $target)
}

# Create packages directory if it does not exist
if (!(Test-Path $PACKAGES_DIRECTORY)) {
    New-Item -ItemType Directory -Path $PACKAGES_DIRECTORY | Out-Null
}

# Download Nuget
if (!(Test-Path $NUGET_TOOL)) {
    Write-Host "Downloading $NUGET_FILE"
    Download-File $NUGET_DOWNLOAD_URL $NUGET_TOOL
}

# Install support packages Boost and OpenSSL
& "$NUGET_TOOL" install hadouken.boost -Version $BOOST_VERSION -OutputDirectory "$PACKAGES_DIRECTORY"
& "$NUGET_TOOL" install hadouken.openssl -Version $OPENSSL_VERSION -OutputDirectory "$PACKAGES_DIRECTORY"
& "$NUGET_TOOL" install hadouken.libtorrent -Version $LIBTORRENT_VERSION -OutputDirectory "$PACKAGES_DIRECTORY"
