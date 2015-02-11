#ifndef HDKN_LOGGER_HPP
#define HDKN_LOGGER_HPP

#include <boost/log/sources/global_logger_storage.hpp>
#include <boost/log/sources/record_ostream.hpp>
#include <boost/log/sources/severity_logger.hpp>
#include <boost/log/trivial.hpp>
#include <boost/log/utility/setup.hpp>

#ifdef WIN32
#define HDKN_API __declspec(dllexport)
#else
#define HDKN_API
#endif

#define HDKN_LOG(lvl) BOOST_LOG_SEV(hadouken::logger::get_logger(), ::boost::log::trivial::lvl)

namespace src = boost::log::sources;

namespace hadouken
{
    class logger
    {
    public:
        static HDKN_API void init();

        static HDKN_API src::severity_logger_mt<boost::log::trivial::severity_level> get_logger();

    private:
        static src::severity_logger_mt<boost::log::trivial::severity_level> lg;
    };
}

#endif
