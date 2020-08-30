#include "session_manager.hpp"

using hadouken::session_manager;

session_manager::session_manager()
{
    m_session = std::make_unique<lt::session>();
}

lt::settings_pack session_manager::settings()
{
    return m_session->get_settings();
}
