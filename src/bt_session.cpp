#include <hadouken/bittorrent/session.hpp>

#include <boost/filesystem.hpp>
#include <boost/filesystem/fstream.hpp>
#include <boost/log/trivial.hpp>

#include <libtorrent/alert_types.hpp>
#include <libtorrent/bencode.hpp>
#include <libtorrent/sha1_hash.hpp>
#include <libtorrent/session.hpp>

#include <hadouken/bittorrent/torrent_handle.hpp>

using namespace hadouken::bittorrent;

session::session(boost::asio::io_service& io_service)
    : io_srv_(io_service)
{
    sess_ = new libtorrent::session();
}

session::~session()
{
    BOOST_LOG_TRIVIAL(trace) << "[i] session::~session()";
    delete sess_;
    BOOST_LOG_TRIVIAL(trace) << "[o] session::~session()";
}

void session::load()
{
    sess_->set_alert_dispatch(boost::bind(&session::alert_dispatch, this, _1));
    sess_->set_alert_mask(libtorrent::alert::all_categories);

    load_state();
    load_resume_data();

    libtorrent::error_code ec;
    sess_->listen_on(std::make_pair(6881, 6889), ec);

    if (ec)
    {
        BOOST_LOG_TRIVIAL(error) << "Could not listen on port: " << ec.message();
    }
}

void session::unload()
{
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

    BOOST_LOG_TRIVIAL(debug) << "Adding torrent file " << file;

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
    // BOOST_LOG_TRIVIAL(trace) << alert->message();

    switch (alert->type())
    {
    case libtorrent::torrent_added_alert::alert_type:
        torrent_added_();
        break;

    case libtorrent::torrent_finished_alert::alert_type:
    {
        libtorrent::torrent_finished_alert* finished_alert = libtorrent::alert_cast<libtorrent::torrent_finished_alert>(alert);
        torrent_handle handle(finished_alert->handle);

        torrent_finished_(handle);
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
    std::ifstream file(".session_state", std::ios::binary);
    if (!file.good()) return;
    
    BOOST_LOG_TRIVIAL(debug) << "Loading session state.";

    file.seekg(0, std::ios::end);
    std::streamsize size = file.tellg();
    file.seekg(0, std::ios::beg);

    std::vector<char> buffer(size);
    file.read(buffer.data(), size);

    libtorrent::error_code ec;
    libtorrent::lazy_entry entry;
    libtorrent::lazy_bdecode(&buffer[0], &buffer[0] + buffer.size(), entry, ec);

    if (ec)
    {
        BOOST_LOG_TRIVIAL(error) << "Could not load session state: " << ec.message();
        return;
    }

    sess_->load_state(entry);
}

void session::load_resume_data()
{
    using namespace boost::filesystem;

    path p("torrents");
    if (!exists(p)) return;

    for (auto entry : directory_iterator(p))
    {
        if (!is_regular_file(entry)) continue;
        if (entry.path().extension() != ".torrent") continue;

        boost::intrusive_ptr<libtorrent::torrent_info> ti = new libtorrent::torrent_info(entry.path().generic_string());

        if (!ti->is_valid())
        {
            BOOST_LOG_TRIVIAL(warning) << "File " << entry.path() << " is not a valid torrent file.";
            continue;
        }

        libtorrent::add_torrent_params params;
        params.ti = ti;
        params.save_path = "C:\\Downloads";

        path resume_data_path = entry.path();
        resume_data_path += ".resume";

        if (exists(resume_data_path))
        {
            BOOST_LOG_TRIVIAL(debug) << "Loading resume data from file: " << resume_data_path;

            ifstream rf(resume_data_path);

            rf.seekg(0, std::ios::end);
            std::streamsize size = rf.tellg();
            rf.seekg(0, std::ios::beg);

            std::vector<char> resume_data(size);
            rf.read(resume_data.data(), size);

            params.resume_data = resume_data;
        }

        sess_->async_add_torrent(params);
    }
}

void session::save_state()
{
    BOOST_LOG_TRIVIAL(debug) << "Saving session state.";

    libtorrent::entry entry;
    sess_->save_state(entry);

    std::vector<char> out;
    libtorrent::bencode(std::back_inserter(out), entry);

    std::ofstream file(".session_state", std::ios::binary);
    file.write(&out[0], out.size());
}

void session::save_resume_data()
{
    sess_->pause();

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

            BOOST_LOG_TRIVIAL(info) << "Saving resume data for " << st.name;

            std::vector<char> out;
            libtorrent::bencode(std::back_inserter(out), *rd->resume_data);

            std::string hash = libtorrent::to_hex(rd->handle.info_hash().to_string());

            boost::filesystem::path p("torrents");
            
            if (!boost::filesystem::exists(p))
            {
                boost::filesystem::create_directory(p);
            }

            boost::filesystem::path file(hash + ".resume");
            
            boost::filesystem::ofstream of(p / file);
            of.write(&out[0], out.size());
        }
    }
}