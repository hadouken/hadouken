#include <Hadouken/Http/HttpSubsystem.hpp>

#include <Hadouken/Http/DefaultRequestHandlerFactory.hpp>
#include <Poco/Net/SecureServerSocket.h>
#include <Poco/Util/LayeredConfiguration.h>

using namespace Hadouken::Http;
using namespace Poco::Util;

HttpSubsystem::HttpSubsystem()
    : port_(7070)
{
}

void HttpSubsystem::initialize(Application& app)
{
    Poco::Net::initializeSSL();

    if (app.config().hasProperty("http.port"))
    {
        port_ = app.config().getInt("http.port");
    }

    if (app.config().hasProperty("http.enable_ssl")
        && app.config().getBool("http.enable_ssl"))
    {
        Poco::Net::SecureServerSocket secureSocket(port_);
        server_ = new Poco::Net::HTTPServer(new DefaultRequestHandlerFactory(), secureSocket, new Poco::Net::HTTPServerParams);
    }
    else
    {
        server_ = new Poco::Net::HTTPServer(new DefaultRequestHandlerFactory(), port_);
    }

    server_->start();

    app.logger().information("HTTP server started on port %i.", port_);
}

void HttpSubsystem::uninitialize()
{
    server_->stop();
    delete server_;

    Poco::Net::uninitializeSSL();
}

const char* HttpSubsystem::name() const
{
    return "HTTP";
}
