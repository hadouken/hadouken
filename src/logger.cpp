#include <hadouken/logger.hpp>
#include <boost/log/expressions.hpp>
#include <boost/log/support/date_time.hpp>
#include <boost/log/utility/setup.hpp>

using namespace hadouken;

boost::log::sources::severity_logger_mt<boost::log::trivial::severity_level> logger::lg;

void logger::init()
{
    namespace expr = boost::log::expressions;

    auto format =
        (
            expr::stream
            << "[" << expr::format_date_time<boost::posix_time::ptime>("TimeStamp", "%H:%M:%S.%f") << "] "
            << "[" << expr::attr<boost::log::attributes::current_thread_id::value_type>("ThreadID") << "] "
            << "[" << boost::log::trivial::severity << "] "
            << expr::smessage
        );

    boost::log::add_console_log
        (
            std::clog,
            boost::log::keywords::format = format
            
        );

    boost::log::add_file_log
        (
            "hadouken.log",
            boost::log::keywords::format = format
        );

    boost::log::add_common_attributes();
}

boost::log::sources::severity_logger_mt<boost::log::trivial::severity_level> logger::get_logger()
{
    return logger::lg;
}