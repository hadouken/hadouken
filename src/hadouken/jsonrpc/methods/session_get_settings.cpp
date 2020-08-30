#include "session_get_settings.hpp"

#include <nlohmann/json.hpp>

#include "../../session_manager.hpp"

using json = nlohmann::json;
using hadouken::jsonrpc::methods::session_get_settings;

session_get_settings::session_get_settings(std::shared_ptr<hadouken::session_manager> session)
    : m_session(session)
{
}

json session_get_settings::execute(json const& args)
{
    auto settings = m_session->settings();

    return {
        { "active_checking",  settings.get_int(lt::settings_pack::active_checking) },
        { "active_dht_limit", settings.get_int(lt::settings_pack::active_dht_limit) },
        { "active_downloads", settings.get_int(lt::settings_pack::active_downloads) },
        { "active_limit",     settings.get_int(lt::settings_pack::active_limit) },
        { "active_lsd_limit", settings.get_int(lt::settings_pack::active_lsd_limit) },
    };
}
