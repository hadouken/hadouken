#include <hadouken/http/gui_request_handler.hpp>

#include <boost/filesystem.hpp>
#include <boost/filesystem/fstream.hpp>
#include <boost/log/trivial.hpp>
#include <hadouken/platform.hpp>

#include "miniz.c"

using namespace hadouken::http;
namespace fs = boost::filesystem;

gui_request_handler::gui_request_handler(const boost::property_tree::ptree& config)
    : config_(config)
{
    fs::path default_path = (hadouken::platform::application_path() / "webui.zip");
    gui_path_ = config_.get<std::string>("http.webui.path", default_path.string());

    use_cache_ = false;
    load_cache();
}

void gui_request_handler::execute(std::string virtual_path,
                                  http_server_t::request const &request,
                                  http_server_t::connection_ptr connection)
{
    std::string file = "index.html";

    if (virtual_path.size() >= 4)
    {
        file = virtual_path.substr(4, std::string::npos);
    }

    if (file[0] == '/')
    {
        file = file.substr(1, std::string::npos);
    }

    if (use_cache_)
    {
        std::map<std::string, std::string>::iterator item = cache_.find(file);
        
        if (item == cache_.end())
        {
            connection->set_status(http_server_t::connection::not_found);
            connection->write(std::string("404 - Not found: '" + file + "'."));

            return;
        }

        http_server_t::response_header headers[] =
        {
            { "Content-Type", get_content_type(fs::path(file).extension().string()) }
        };

        connection->set_status(http_server_t::connection::ok);
        connection->set_headers(boost::make_iterator_range(headers, headers + 1));
        connection->write(item->second);

        return;
    }

    fs::path resource_path = gui_path_ / file;

    if (!fs::exists(resource_path))
    {
        connection->set_status(http_server_t::connection::not_found);
        connection->write(std::string("404 - Not found."));
    }
    else
    {
        fs::ifstream in(resource_path, std::ios::binary);
        std::stringstream buf;
        buf << in.rdbuf();

        http_server_t::response_header headers[] =
        {
            { "Content-Type", get_content_type(resource_path.extension().string()) }
        };

        connection->set_status(http_server_t::connection::ok);
        connection->set_headers(boost::make_iterator_range(headers, headers + 1));
        connection->write(buf.str());
    }
}

std::string gui_request_handler::get_content_type(std::string ext)
{
    if (ext == ".css") { return "text/css"; }
    if (ext == ".js") { return "text/javascript"; }
    if (ext == ".png") { return "image/png"; }
    return "text/html";
}

void gui_request_handler::load_cache()
{
    if (gui_path_.empty()
        || !gui_path_.has_extension()
        || gui_path_.extension() != ".zip")
    {
        return;
    }

    BOOST_LOG_TRIVIAL(info) << "Loading webui cache from " << gui_path_ << ".";

    mz_zip_archive* archive = new mz_zip_archive();
    mz_bool status = mz_zip_reader_init_file(archive, gui_path_.string().c_str(), 0);

    if (!status)
    {
        BOOST_LOG_TRIVIAL(error) << "Could not open webui archive.";
        return;
    }

    mz_uint file_count = mz_zip_reader_get_num_files(archive);

    for (mz_uint i = 0; i < file_count; i++)
    {
        mz_zip_archive_file_stat* stat = new mz_zip_archive_file_stat();
        status = mz_zip_reader_file_stat(archive, i, stat);

        if (!status)
        {
            BOOST_LOG_TRIVIAL(error) << "Could not stat file index " << i;
            continue;
        }

        size_t size;
        char* data = static_cast<char*>(mz_zip_reader_extract_file_to_heap(archive, stat->m_filename, &size, 0));

        std::string cache(data, size);
        std::string name(stat->m_filename);

        cache_.insert(std::make_pair(name, cache));
        delete stat;
    }

    mz_zip_reader_end(archive);
    delete archive;

    BOOST_LOG_TRIVIAL(debug) << cache_.size() << " items in webui cache.";
    use_cache_ = true;
}
