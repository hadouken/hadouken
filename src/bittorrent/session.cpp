#include <hadouken/bittorrent/session.hpp>

#include <boost/date_time.hpp>
#include <boost/filesystem.hpp>
#include <boost/filesystem/fstream.hpp>
#include <boost/regex.hpp>

#include <libtorrent/alert_types.hpp>
#include <libtorrent/bencode.hpp>
#include <libtorrent/sha1_hash.hpp>
#include <libtorrent/session.hpp>

#include <hadouken/bittorrent/torrent_handle.hpp>
#include <hadouken/logger.hpp>

namespace fs = boost::filesystem;
namespace pt = boost::property_tree;
using namespace hadouken::bittorrent;

session::session(const boost::property_tree::ptree& config, boost::asio::io_service& io_service)
    : config_(config),
      io_srv_(io_service),
      auto_add_timer_(io_service, boost::posix_time::seconds(1))
{
    sess_ = new libtorrent::session();

    pt::ptree session_config = config_.get_child("session");

    default_save_path_ = session_config.get<std::string>("default_save_path");
    state_path_ = session_config.get<std::string>("state_path");
}

session::~session()
{
    HDKN_LOG(trace) << "Disposing session.";
    delete sess_;
    HDKN_LOG(trace) << "Session disposed.";
}

void session::load()
{
    sess_->set_alert_dispatch(boost::bind(&session::alert_dispatch, this, _1));
    sess_->set_alert_mask(libtorrent::alert::all_categories);

    pt::ptree session_config = config_.get_child("session");

    auto port_it = session_config.get_child("listen_port_range").begin();
    int port_min = (port_it)->second.get_value<int>();
    int port_max = (++port_it)->second.get_value<int>();

    load_state();
    load_resume_data();

    // load auto add and auto move
    load_auto_add_rules();
    load_auto_move_rules();

    HDKN_LOG(debug) << "Starting session in port range [" << port_min << "-" << port_max << "].";

    libtorrent::error_code ec;
    sess_->listen_on(std::make_pair(port_min, port_max), ec);

    if (ec)
    {
        HDKN_LOG(error) << "Could not listen on port: " << ec.message();
    }

    HDKN_LOG(info) << "Session started on port " << sess_->listen_port() << ".";

    // start the auto add timer
    auto_add_timer_.async_wait(boost::bind(&session::check_auto_add_folders, this, _1));
}

void session::unload()
{
    // Cancel the timer, otherwise the io service loop
    // will never complete.
    auto_add_timer_.cancel();

    typedef boost::function<void(std::auto_ptr<libtorrent::alert>)> dispatch_function_t;
    sess_->set_alert_dispatch(dispatch_function_t());

    save_state();
    save_resume_data();
}

void session::add_torrent_file(const std::string& file, const std::string& save_path)
{
    libtorrent::add_torrent_params params;
    params.save_path = save_path;
    params.ti = new libtorrent::torrent_info(file);

    HDKN_LOG(debug) << "Adding torrent file " << params.ti->name() << " [" << libtorrent::to_hex(params.ti->info_hash().to_string()) << "].";

    sess_->async_add_torrent(params);
}

boost::signals2::connection session::on_torrent_added(const torrent_added_t::slot_type &subscriber)
{
    return torrent_added_.connect(subscriber);
}

boost::signals2::connection session::on_torrent_finished(const torrent_finished_t::slot_type &subscriber)
{
    return torrent_finished_.connect(subscriber);
}

void session::alert_dispatch(std::auto_ptr<libtorrent::alert> alert_ptr)
{
    auto alert = alert_ptr.get();
    alert_ptr.release();

    io_srv_.dispatch(boost::bind(&session::handle_alert, this, alert));
}

void session::handle_alert(libtorrent::alert* alert)
{
    //HDKN_LOG(trace) << alert->message();

    switch (alert->type())
    {
    case libtorrent::torrent_added_alert::alert_type:
        torrent_added_();
        break;

    case libtorrent::torrent_finished_alert::alert_type:
    {
        libtorrent::torrent_finished_alert* finished_alert = libtorrent::alert_cast<libtorrent::torrent_finished_alert>(alert);
        libtorrent::torrent_handle handle = finished_alert->handle;
        std::string name = handle.status(libtorrent::torrent_handle::query_name).name;
        
        HDKN_LOG(info) << "Torrent " << name << " finished downloading.";

        for (auto rule : auto_move_rules_)
        {
            boost::regex re(rule.pattern, boost::regex_constants::icase);

            if (boost::regex_match(name, re))
            {
                HDKN_LOG(info) << "Moving torrent to " << rule.path << ".";
                handle.move_storage(rule.path);

                break;
            }
        }
    }
        break;

    case libtorrent::torrent_removed_alert::alert_type:
        torrent_removed_();
        break;
    }

    delete alert;
}

void session::load_state()
{
    fs::path state_path = get_state_path();
    if (state_path.empty()) return;

    fs::path state_file(state_path / ".session_state");
    if (!fs::exists(state_file)) return;


    HDKN_LOG(debug) << "Loading session state from " << state_file << ".";

    fs::ifstream state_file_stream(state_file);
    std::streamsize size = fs::file_size(state_file);
    std::vector<char> buffer(size);

    state_file_stream.read(buffer.data(), size);

    libtorrent::error_code ec;
    libtorrent::lazy_entry entry;
    libtorrent::lazy_bdecode(&buffer[0], &buffer[0] + buffer.size(), entry, ec);

    if (ec)
    {
        HDKN_LOG(error) << "Could not load session state: " << ec.message();
        return;
    }

    sess_->load_state(entry);
}

void session::load_resume_data()
{
    fs::path state_path = get_state_path();
    if (state_path.empty()) return;

    fs::path torrents_path(state_path / "torrents");
    if (!fs::exists(torrents_path)) return;

    fs::directory_iterator end;

    for (fs::directory_iterator entry(torrents_path); entry != end; ++entry)
    {
        if (!is_regular_file(*entry)) continue;
        if (entry->path().extension() != ".torrent") continue;

        boost::intrusive_ptr<libtorrent::torrent_info> ti = new libtorrent::torrent_info(entry->path().generic_string());

        if (!ti->is_valid())
        {
            HDKN_LOG(warning) << "File " << entry->path() << " is not a valid torrent file.";
            continue;
        }

        libtorrent::add_torrent_params params;
        params.ti = ti;
        params.save_path = default_save_path_;

        fs::path resume_data_path = entry->path();
        resume_data_path += ".resume";

        if (exists(resume_data_path))
        {
            HDKN_LOG(debug) << "Loading resume data from file: " << resume_data_path;

            fs::ifstream rf(resume_data_path);
            std::streamsize size = fs::file_size(resume_data_path);

            std::vector<char> resume_data(size);
            rf.read(resume_data.data(), size);

            params.resume_data = resume_data;
        }

        sess_->async_add_torrent(params);
    }
}

void session::load_auto_add_rules()
{
    if (!config_.count("auto_add")) return;

    for (auto item : config_.get_child("auto_add"))
    {
        auto_add_rule rule;
        rule.pattern = item.second.get<std::string>("pattern");
        rule.save_path = item.second.get<std::string>("save_path");
        rule.source_path = item.second.get<std::string>("source_path");

        auto_add_rules_.push_back(rule);
    }

    HDKN_LOG(info) << "Added " << auto_add_rules_.size() << " auto add rule(s).";
}

void session::load_auto_move_rules()
{
    if (!config_.count("auto_move")) return;

    for (auto item : config_.get_child("auto_move"))
    {
        auto_move_rule rule;
        rule.input = item.second.get<std::string>("match_input");
        rule.path = item.second.get<std::string>("destination_path");
        rule.pattern = item.second.get<std::string>("match_pattern");

        auto_move_rules_.push_back(rule);
    }

    HDKN_LOG(info) << "Added " << auto_move_rules_.size() << " auto move rule(s).";
}

void session::save_state()
{
    HDKN_LOG(debug) << "Saving session state.";

    libtorrent::entry entry;
    sess_->save_state(entry);

    std::vector<char> out;
    libtorrent::bencode(std::back_inserter(out), entry);

    fs::path state_path = get_state_path();
    if (state_path.empty()) return;

    fs::path state_file(state_path / ".session_state");

    fs::ofstream file(state_file, std::ios::binary);
    file.write(&out[0], out.size());
}

void session::save_resume_data()
{
    sess_->pause();

    fs::path state_path = get_state_path();
    if (state_path.empty()) return;

    fs::path torrents_path(state_path / "torrents");

    if (!fs::exists(torrents_path))
    {
        fs::create_directory(torrents_path);
    }

    int num = 0;

    std::vector<libtorrent::torrent_status> temp;
    sess_->get_torrent_status(&temp, [](const libtorrent::torrent_status) { return true; });

    for (auto &status : temp)
    {
        if (!status.handle.is_valid()) continue;
        if (!status.has_metadata) continue;
        if (!status.need_save_resume) continue;

        ++num;
        status.handle.save_resume_data();
    }

    HDKN_LOG(info) << "Saving resume data for " << num << " torrents.";

    while (num > 0)
    {
        libtorrent::alert const* alert = sess_->wait_for_alert(libtorrent::seconds(10));
        if (alert == 0) continue;

        std::deque<libtorrent::alert*> alerts;
        sess_->pop_alerts(&alerts);

        for (auto &alert : alerts)
        {
            std::auto_ptr<libtorrent::alert> a(alert);

            if (libtorrent::alert_cast<libtorrent::torrent_paused_alert>(alert)) continue;

            if (libtorrent::alert_cast<libtorrent::save_resume_data_failed_alert>(alert))
            {
                --num;
                continue;
            }

            const libtorrent::save_resume_data_alert* rd = libtorrent::alert_cast<libtorrent::save_resume_data_alert>(alert);
            if (!rd) continue;

            --num;

            if (!rd->resume_data) continue;

            libtorrent::torrent_status st = rd->handle.status(libtorrent::torrent_handle::status_flags_t::query_name);

            HDKN_LOG(info) << "Saving resume data for " << st.name;

            std::vector<char> out;
            libtorrent::bencode(std::back_inserter(out), *rd->resume_data);

            std::string hash = libtorrent::to_hex(rd->handle.info_hash().to_string());
            fs::path file_path(hash + ".resume");
            
            fs::ofstream file_stream(torrents_path / file_path);
            file_stream.write(&out[0], out.size());
        }
    }
}

void session::check_auto_add_folders(const boost::system::error_code& error)
{
    if (error && error == boost::asio::error::operation_aborted)
    {
        return;
    }

    for (auto rule : auto_add_rules_)
    {
        fs::path source_path(rule.source_path);
        if (!fs::exists(source_path)) continue;

        if (!fs::is_directory(source_path))
        {
            HDKN_LOG(warning) << "Source path is not a directory. Path: " << source_path << ".";
            continue;
        }

        std::string save_path = default_save_path_;

        if (!rule.save_path.empty())
        {
            save_path = rule.save_path;
        }

        fs::directory_iterator end_iter;

        for (fs::directory_iterator entry(source_path); entry != end_iter; ++entry)
        {
            if (!fs::is_regular_file(entry->path())) continue;

            if (rule.pattern.empty() && entry->path().extension() == ".torrent")
            {
                // if no pattern is specified, only add .torrent files.
                add_torrent_file(entry->path().string(), save_path);
            }
            else
            {
                boost::regex re(rule.pattern);
                if (!boost::regex_match(entry->path().filename().string(), re)) continue;

                // add file
                add_torrent_file(entry->path().string(), save_path);
            }

            fs::remove(entry->path());
        }
    }

    auto_add_timer_.expires_at(auto_add_timer_.expires_at() + boost::posix_time::seconds(5));
    auto_add_timer_.async_wait(boost::bind(&session::check_auto_add_folders, this, _1));
}

fs::path session::get_state_path()
{
    fs::path state_path(state_path_);

    if (!fs::is_directory(state_path) && fs::exists(state_path))
    {
        HDKN_LOG(error) << "The configured state path exists but is not a directory. Path: " << state_path;
        return fs::path();
    }

    if (!fs::exists(state_path))
    {
        HDKN_LOG(debug) << "Creating state directory. Path: " << state_path;
        fs::create_directories(state_path);
    }

    return state_path;
}