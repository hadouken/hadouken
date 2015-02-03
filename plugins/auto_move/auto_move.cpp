#include "auto_move.hpp"

#include <boost/log/trivial.hpp>

auto_move::auto_move(hadouken::service_locator& service_locator)
    : plugin(service_locator)
{

}

void auto_move::load()
{
    BOOST_LOG_TRIVIAL(debug) << "Loading auto move";
}