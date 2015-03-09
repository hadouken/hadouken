#ifndef AUTOADD_AUTOADDEXTENSION_HPP
#define AUTOADD_AUTOADDEXTENSION_HPP

#include <Hadouken/Extensions/Extension.hpp>

#include "folder.hpp"
#include <Poco/Logger.h>
#include <Poco/Timer.h>
#include <Poco/Util/AbstractConfiguration.h>
#include <vector>

namespace AutoAdd
{
    class AutoAddExtension : public Hadouken::Extensions::Extension
    {
    public:
        AutoAddExtension();

        void load(Poco::Util::AbstractConfiguration& config);

        void unload();

    private:
        void monitor(Poco::Timer& timer);

        Poco::Logger& logger_;
        Poco::Timer monitor_;
        std::vector<Folder> folders_;
    };
}

#endif
