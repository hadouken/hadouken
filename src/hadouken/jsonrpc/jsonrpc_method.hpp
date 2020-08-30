#pragma once

#include <nlohmann/json.hpp>

using json = nlohmann::json;

namespace hadouken
{
namespace jsonrpc
{
    class jsonrpc_method
    {
    public:
        virtual json execute(json const& args) = 0;
    };
}
}
