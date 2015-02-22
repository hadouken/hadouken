#include <Hadouken/Http/HttpSubsystem.hpp>
#include <Hadouken/Http/DefaultRequestHandlerFactory.hpp>
#include <Poco/Util/LayeredConfiguration.h>

using namespace Hadouken::Http;
using namespace Poco::Util;

HttpSubsystem::HttpSubsystem()
    : port_(7070)
{
}

void HttpSubsystem::initialize(Application& app)
{
    if (app.config().hasProperty("http.port"))
    {
        port_ = app.config().getInt("http.port");
    }

    server_ = new Poco::Net::HTTPServer(new DefaultRequestHandlerFactory(), port_);
    server_->start();

    app.logger().information("HTTP server started on port %i.", port_);
}

void HttpSubsystem::uninitialize()
{
    server_->stop();
    delete server_;
}

const char* HttpSubsystem::name() const
{
    return "HTTP";
}
