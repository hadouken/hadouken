#include "torrents_add.hpp"

#include <nlohmann/json.hpp>

using json = nlohmann::json;
using hadouken::jsonrpc::methods::torrents_add;

torrents_add::torrents_add(std::shared_ptr<hadouken::session_manager> session)
    : m_session(session)
{
}

json torrents_add::execute(json const& args)
{
    // if string and length is 40 and hex - add as info hash
    // if string and starts with magnet:? - add as magnet link

    return {"hej", 123};
}
