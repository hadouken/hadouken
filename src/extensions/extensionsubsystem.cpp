#include <Hadouken/Extensions/ExtensionSubsystem.hpp>

#include <iostream>
#include <Hadouken/Extensions/Extension.hpp>
#include <Hadouken/Platform.hpp>
#include <Poco/File.h>
#include <Poco/Path.h>

using namespace Hadouken::Extensions;
using namespace Poco::Util;

ExtensionSubsystem::ExtensionSubsystem()
    : logger_(Poco::Logger::get("hadouken.extensions"))
{
}

void ExtensionSubsystem::initialize(Application& app)
{
    Poco::Util::AbstractConfiguration::Keys keys;
    app.config().keys("extensions", keys);

    for (auto k : keys)
    {
        AbstractConfiguration* pluginConfig = app.config().createView("extensions." + k);
        loadExtension(k, *pluginConfig);
    }

    for (auto extensionLoader : loader_)
    {
        logger_.information("Loading extensions from '%s'.", extensionLoader->first);

        for (auto manifest : *extensionLoader->second)
        {
            logger_.information("Found manifest '%s'", std::string(manifest->name()));

            Extension* extension = manifest->create();
            extension->load(app.config());

            extensions_.push_back(extension);
        }
    }
}

void ExtensionSubsystem::uninitialize()
{
    for (auto ext : extensions_)
    {
        ext->unload();
        delete ext;
    }

    for (auto lib : libs_)
    {
        loader_.unloadLibrary(lib);
    }
}

const char* ExtensionSubsystem::name() const
{
    return "Extensions";
}

void ExtensionSubsystem::loadExtension(std::string extensionName, AbstractConfiguration& config)
{
    // Only load extensions which are excplicitly enabled, eg. "enabled": true.
    if (!config.getBool("enabled", false))
    {
        return;
    }

    std::string libraryName = getLibraryName(extensionName);

    Poco::Path applicationPath = Hadouken::Platform::getApplicationPath();
    Poco::File libraryFile = Poco::Path(applicationPath, libraryName);

    try
    {
        loader_.loadLibrary(libraryFile.path());
        libs_.push_back(libraryFile.path());
    }
    catch (Poco::Exception& loaderException)
    {
        logger_.error("%s", loaderException.displayText());
    }
}

std::string ExtensionSubsystem::getLibraryName(std::string extensionName)
{
    // Windows extensions are named, for example, autoadd.dll.
    // However, building on Linux gives a filename of libautoadd.so.
    // Adjust accordingly.

#if defined(WIN32)
    return extensionName + ".dll";
#elif defined(__APPLE__)
    return ".dylib";
#else
    return "lib" + extensionName + ".so";
#endif
}
