#pragma once

#include <memory>

#include <nlohmann/json.hpp>

#include "../jsonrpc_method.hpp"
#include "../../session_manager.hpp"

using json = nlohmann::json;

namespace hadouken
{
namespace jsonrpc
{
namespace methods
{
    class torrents_add : public jsonrpc_method
    {
    public:
        torrents_add(std::shared_ptr<hadouken::session_manager> session);
        json execute(json const& args);

    private:
        std::shared_ptr<hadouken::session_manager> m_session;
    };
}
}
}
