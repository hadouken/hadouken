#include <hadouken/application.hpp>

#include <boost/filesystem.hpp>
#include <boost/log/trivial.hpp>
#include <hadouken/platform.hpp>
#include <libtorrent/session.hpp>
#include <libtorrent/extensions/lt_trackers.hpp>
#include <libtorrent/extensions/smart_ban.hpp>
#include <libtorrent/extensions/ut_metadata.hpp>
#include <libtorrent/extensions/ut_pex.hpp>

using namespace hadouken;
namespace fs = boost::filesystem;
typedef boost::function<void(std::auto_ptr<libtorrent::alert>)> dispatch_function_t;

application::application(boost::shared_ptr<boost::asio::io_service> io, const pt::ptree& config)
    : io_(io),
    config_(config)
{
    script_host_ = std::make_unique<hadouken::scripting::script_host>(*this);
    
    http_ = std::make_unique<hadouken::http::http_server>(io, config);
    http_->set_auth_callback(boost::bind(&application::is_authenticated, this, _1));
    http_->set_rpc_callback(boost::bind(&application::rpc, this, _1));

    libtorrent::fingerprint fingerprint("LT", LIBTORRENT_VERSION_MAJOR, LIBTORRENT_VERSION_MINOR, 0, 0);
    session_ = std::make_unique<libtorrent::session>(fingerprint, 0);
}

application::~application()
{
}

void application::start()
{
    // session settings
    session_->set_alert_mask(libtorrent::alert::category_t::all_categories);
    session_->set_alert_dispatch(std::bind(&application::alert_dispatch, this, std::placeholders::_1));

    // session extensions
    session_->add_extension(&libtorrent::create_lt_trackers_plugin);
    session_->add_extension(&libtorrent::create_smart_ban_plugin);
    session_->add_extension(&libtorrent::create_ut_metadata_plugin);
    session_->add_extension(&libtorrent::create_ut_pex_plugin);

    // load scripting host
    fs::path defaultPath = (hadouken::platform::application_path() / "js");
    fs::path scriptPath = config_.get<std::string>("scripting.path", defaultPath.string());
    script_host_->load(scriptPath);

    http_->start();
}

void application::stop()
{
    http_->stop();
    session_->set_alert_dispatch(dispatch_function_t());
    script_host_->unload();
}

libtorrent::session& application::session() const
{
    return *session_;
}

hadouken::scripting::script_host& application::script_host() const
{
    return *script_host_;
}

void application::alert_dispatch(std::auto_ptr<libtorrent::alert> alert_ptr)
{
    io_->dispatch(std::bind(&application::alert_handler, this, alert_ptr.release()));
}

void application::alert_handler(libtorrent::alert* alert)
{
    script_host_->emit(alert->what(), alert);
}

bool application::is_authenticated(std::string auth_header)
{
    return script_host_->is_authenticated(auth_header);
}

std::string application::rpc(std::string data)
{
    return script_host_->rpc(data);
}
