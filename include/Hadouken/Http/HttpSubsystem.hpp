#ifndef HADOUKEN_HTTP_SUBSYSTEM_HPP
#define HADOUKEN_HTTP_SUBSYSTEM_HPP

#include <Hadouken/Config.hpp>
#include <Poco/Logger.h>
#include <Poco/Net/HTTPServer.h>
#include <Poco/Util/Application.h>
#include <Poco/Util/Subsystem.h>

#include <memory>

using namespace Poco::Util;

namespace Hadouken
{
    namespace Http
    {
        class HttpSubsystem : public Subsystem
        {
        public:
            HDKN_EXPORT HttpSubsystem();

        protected:
            void initialize(Application& app);

            void uninitialize();

            const char* name() const;

        private:
            Poco::Net::ServerSocket getServerSocket(Application& app);
            Poco::Logger& logger_;
            int port_;
            std::unique_ptr<Poco::Net::HTTPServer> server_;
        };
    }
}

#endif