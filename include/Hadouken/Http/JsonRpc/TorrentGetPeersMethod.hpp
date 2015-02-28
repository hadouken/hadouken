#ifndef HADOUKEN_HTTP_JSONRPC_TORRENTGETPEERSMETHOD_HPP
#define HADOUKEN_HTTP_JSONRPC_TORRENTGETPEERSMETHOD_HPP

#include <Hadouken/Http/JsonRpc/RpcMethod.hpp>

namespace Hadouken
{
    namespace Http
    {
        namespace JsonRpc
        {
            class TorrentGetPeersMethod : public RpcMethod
            {
            public:
                Poco::Dynamic::Var::Ptr execute(const Poco::JSON::Array::Ptr& params);
            };
        }
    }
}

#endif
