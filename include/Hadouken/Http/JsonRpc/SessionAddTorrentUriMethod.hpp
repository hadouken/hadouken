#ifndef HADOUKEN_HTTP_JSONRPC_SESSIONADDTORRENTURIMETHOD_HPP
#define HADOUKEN_HTTP_JSONRPC_SESSIONADDTORRENTURIMETHOD_HPP

#include <Hadouken/Http/JsonRpc/RpcMethod.hpp>
#include <Poco/JSON/Array.h>

namespace Hadouken
{
    namespace Http
    {
        namespace JsonRpc
        {
            class SessionAddTorrentUriMethod : public RpcMethod
            {
            public:
                Poco::Dynamic::Var::Ptr execute(const Poco::JSON::Array::Ptr& params);                
            };
        }
    }
}

#endif
