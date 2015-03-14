#ifndef HADOUKEN_EXTENSIONSUBSYSTEM_HPP
#define HADOUKEN_EXTENSIONSUBSYSTEM_HPP

#include <Hadouken/Config.hpp>
#include <Poco/ClassLoader.h>
#include <Poco/Logger.h>
#include <Poco/Manifest.h>
#include <Poco/Util/Application.h>
#include <Poco/Util/Subsystem.h>

using namespace Poco::Util;

namespace Hadouken
{
    namespace Extensions
    {
        class Extension;

        typedef Poco::ClassLoader<Extension> ExtensionLoader;
        typedef Poco::Manifest<Extension> ExtensionManifest;

        class ExtensionSubsystem : public Subsystem
        {
        public:
            HDKN_EXPORT ExtensionSubsystem();
            
        protected:
            HDKN_EXPORT void initialize(Application& app);

            HDKN_EXPORT void uninitialize();

            HDKN_EXPORT const char* name() const;

        private:
            std::string getLibrarySuffix();
            void loadExtension(std::string extensionName, AbstractConfiguration& config);

            Poco::Logger& logger_;
            ExtensionLoader loader_;
            std::vector<std::string> libs_;
            std::vector<Extension*> extensions_;
        };
    }
}

#endif
