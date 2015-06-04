#ifndef HADOUKEN_LOGGING_HPP
#define HADOUKEN_LOGGING_HPP

#include <boost/core/null_deleter.hpp>
#include <boost/filesystem.hpp>
#include <boost/log/core.hpp>
#include <boost/log/expressions.hpp>
#include <boost/log/trivial.hpp>
#include <boost/log/expressions.hpp>
#include <boost/log/sinks/text_file_backend.hpp>
#include <boost/log/sinks/text_ostream_backend.hpp>
#include <boost/log/utility/setup/file.hpp>
#include <boost/log/utility/setup/common_attributes.hpp>
#include <boost/log/sources/severity_logger.hpp>
#include <boost/log/sources/record_ostream.hpp>
#include <boost/log/support/date_time.hpp>
#include <boost/program_options.hpp>
#include <hadouken/platform.hpp>

namespace fs = boost::filesystem;
namespace attrs = boost::log::attributes;
namespace src = boost::log::sources;
namespace expr = boost::log::expressions;
namespace sinks = boost::log::sinks;
namespace keywords = boost::log::keywords;
namespace po = boost::program_options;

namespace hadouken
{
    class logging
    {
    public:
        static void setup(const po::variables_map& options)
        {
            boost::shared_ptr<boost::log::core> core = boost::log::core::get();
            boost::log::add_common_attributes();

            // Create backend and attach streams
            boost::shared_ptr<sinks::text_ostream_backend> backend = boost::make_shared<sinks::text_ostream_backend>();
            backend->add_stream(boost::shared_ptr<std::ostream>(&std::clog, boost::null_deleter()));

            // File logging should only be added if we have the "daemon" option or
            // if we explicitly specify "log-file".
            if (options.count("daemon"))
            {
                fs::path file = (hadouken::platform::data_path() / "hadouken.log");
                backend->add_stream(boost::make_shared<std::ofstream>(file.string()));
            }
            else if (options.count("log-file"))
            {
                std::string file = options["log-file"].as<std::string>();
                backend->add_stream(boost::make_shared<std::ofstream>(file));
            }

            // Enable auto-flushing
            backend->auto_flush(true);

            // Wrap it into the frontend and register in the core.
            // The backend requires synchronization in the frontend.
            typedef sinks::synchronous_sink<sinks::text_ostream_backend> sink_t;
            boost::shared_ptr<sink_t> sink(new sink_t(backend));

            sink->set_formatter
                (
                expr::stream
                << "(" << std::hex << std::setw(8) << std::setfill('0') << expr::attr<unsigned int>("LineID") << ") "
                << expr::format_date_time<boost::posix_time::ptime>("TimeStamp", "%H:%M:%S.%f") << ": "
                << "<" << boost::log::trivial::severity << "> "
                << expr::smessage
                );

            core->add_sink(sink);
        }
    };
}

#endif
