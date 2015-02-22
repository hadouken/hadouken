#ifndef HADOUKEN_HTTP_SUBSYSTEM_HPP
#define HADOUKEN_HTTP_SUBSYSTEM_HPP

#include <Poco/Net/HTTPServer.h>
#include <Poco/Util/Application.h>
#include <Poco/Util/Subsystem.h>

using namespace Poco::Util;

namespace Hadouken
{
    namespace Http
    {
        class HttpSubsystem : public Subsystem
        {
        public:
            HttpSubsystem();

        protected:
            void initialize(Application& app);

            void uninitialize();

            const char* name() const;

        private:
            int port_;
            Poco::Net::HTTPServer* server_;
        };
    }
}

#endif