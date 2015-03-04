#ifndef HADOUKEN_HTTP_JSONRPC_RPCMETHOD_HPP
#define HADOUKEN_HTTP_JSONRPC_RPCMETHOD_HPP

#include <Poco/JSON/Array.h>
#include <Poco/JSON/Object.h>

namespace Hadouken
{
    namespace Http
    {
        namespace JsonRpc
        {
            class RpcMethod
            {
            public:
                virtual Poco::Dynamic::Var::Ptr execute(const Poco::JSON::Array::Ptr& params) = 0;
            };
        }
    }
}

#endif
