#include <Hadouken/Extensions/ExtensionSubsystem.hpp>

#include <iostream>
#include <Hadouken/Extensions/Extension.hpp>

using namespace Hadouken::Extensions;
using namespace Poco::Util;

std::string librarySuffix()
{
#if defined(WIN32)
    return ".dll";
#elif defined(__APPLE__)
    return ".dylib";
#else
    return ".so";
#endif
}

ExtensionSubsystem::ExtensionSubsystem()
    : logger_(Poco::Logger::get("hadouken.extensions"))
{
}

void ExtensionSubsystem::initialize(Application& app)
{
    std::string extName = "autoadd" + librarySuffix();

    try
    {
        logger_.information("Loading extension '%s'.", extName);
        loader_.loadLibrary(extName);
    }
    catch (Poco::Exception& loaderException)
    {
        logger_.error("%s", loaderException.displayText());
        return;
    }

    libs_.push_back(extName);

    ExtensionLoader::Iterator it(loader_.begin());
    ExtensionLoader::Iterator end(loader_.end());

    for (; it != end; ++it)
    {
        ExtensionManifest::Iterator itMan(it->second->begin());
        ExtensionManifest::Iterator endMan(it->second->end());

        for (; itMan != endMan; ++itMan)
        {
            std::cout << itMan->name() << std::endl;

            itMan->create()->load();
        }
    }
}

void ExtensionSubsystem::uninitialize()
{
    for (auto lib : libs_)
    {
        loader_.unloadLibrary(lib);
    }
}

const char* ExtensionSubsystem::name() const
{
    return "Extensions";
}
