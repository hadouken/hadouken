#include <Hadouken/Extensions/ExtensionSubsystem.hpp>

#include <iostream>
#include <Hadouken/Extensions/Extension.hpp>

using namespace Hadouken::Extensions;
using namespace Poco::Util;

ExtensionSubsystem::ExtensionSubsystem()
    : logger_(Poco::Logger::get("hadouken.extensions"))
{
}

void ExtensionSubsystem::initialize(Application& app)
{
    Poco::Util::AbstractConfiguration::Keys keys;
    app.config().keys("plugins", keys);

    for (auto k : keys)
    {
        AbstractConfiguration* pluginConfig = app.config().createView("plugins." + k);
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

    if (!config.hasProperty("enabled")
        || !config.getBool("enabled"))
    {
        return;
    }

    std::string libraryPath = extensionName + getLibrarySuffix();

    try
    {
        loader_.loadLibrary(libraryPath);
        libs_.push_back(libraryPath);
    }
    catch (Poco::Exception& loaderException)
    {
        logger_.error("%s", loaderException.displayText());
    }
}

std::string ExtensionSubsystem::getLibrarySuffix()
{
#if defined(WIN32)
    return ".dll";
#elif defined(__APPLE__)
    return ".dylib";
#else
    return ".so";
#endif
}
