#include "torrent_engine.hpp"

#include <boost/filesystem.hpp>
#include <boost/filesystem/fstream.hpp>
#include <boost/log/trivial.hpp>

#include <libtorrent/alert_types.hpp>
#include <libtorrent/bencode.hpp>
#include <libtorrent/sha1_hash.hpp>
#include <libtorrent/session.hpp>

using namespace hadouken;

torrent_engine::torrent_engine()
{
    session_ = new libtorrent::session();
}

torrent_engine::~torrent_engine()
{
    delete session_;
}

void torrent_engine::load()
{
    load_state();
    load_resume_data();
}

void torrent_engine::unload()
{
    save_state();
    save_resume_data();
}

void torrent_engine::load_state()
{
    std::ifstream file(".session_state", std::ios::binary);
    if (!file.good()) return;
    
    BOOST_LOG_TRIVIAL(debug) << "Loading session state.";

    file.seekg(0, std::ios::end);
    std::streamsize size = file.tellg();
    file.seekg(0, std::ios::beg);

    std::vector<char> buffer(size);
    file.read(buffer.data(), size);

    libtorrent::lazy_entry entry;
    libtorrent::lazy_bdecode(&buffer[0], &buffer[0] + buffer.size(), entry);

    session_->load_state(entry);
}

void torrent_engine::load_resume_data()
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

        path resume_data = entry.path();
        resume_data += ".resume";

        if (exists(resume_data))
        {
            ifstream rf(resume_data);

            rf.seekg(0, std::ios::end);
            std::streamsize size = rf.tellg();
            rf.seekg(0, std::ios::beg);

            std::vector<char> resume(size);
            rf.read(resume.data(), size);

            params.resume_data = resume;
        }

        session_->async_add_torrent(params);
    }
}

void torrent_engine::save_state()
{
    BOOST_LOG_TRIVIAL(debug) << "Saving session state.";

    libtorrent::entry entry;
    session_->save_state(entry);

    std::vector<char> out;
    libtorrent::bencode(std::back_inserter(out), entry);

    std::ofstream file(".session_state");
    file.write(&out[0], out.size());
}

void torrent_engine::save_resume_data()
{
    session_->pause();

    int num = 0;

    std::vector<libtorrent::torrent_status> temp;
    session_->get_torrent_status(&temp, [](const libtorrent::torrent_status) { return true; });

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
        libtorrent::alert const* alert = session_->wait_for_alert(libtorrent::seconds(10));
        if (alert == 0) continue;

        std::deque<libtorrent::alert*> alerts;
        session_->pop_alerts(&alerts);

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

            BOOST_LOG_TRIVIAL(info) << "Saving resume data for " << rd->handle.name();

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