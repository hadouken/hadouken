#ifndef HADOUKEN_HTTP_JSONRPCREQUESTHANDLER_HPP
#define HADOUKEN_HTTP_JSONRPCREQUESTHANDLER_HPP

#include <Poco/Net/HTTPRequestHandler.h>
#include <Poco/Util/AbstractConfiguration.h>

#include <memory>

using namespace Poco::Net;

namespace Hadouken
{
    namespace Http
    {
        class JsonRpcRequestHandler : public HTTPRequestHandler
        {
        public:
            JsonRpcRequestHandler(const Poco::Util::AbstractConfiguration& config);

            void handleRequest(HTTPServerRequest& request, HTTPServerResponse& response);

        protected:
            bool isValidRequest(HTTPServerRequest& request) const;

        private:
            const Poco::Util::AbstractConfiguration& config_;
        };
    }
}

#endif
