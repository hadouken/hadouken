#include <Hadouken/Http/HttpSubsystem.hpp>

#include <Hadouken/Http/DefaultRequestHandlerFactory.hpp>
#include <Poco/Net/SecureServerSocket.h>
#include <Poco/Util/LayeredConfiguration.h>

using namespace Hadouken::Http;
using namespace Poco::Net;
using namespace Poco::Util;

HttpSubsystem::HttpSubsystem()
    : port_(7070),
      logger_(Poco::Logger::get("hadouken.http"))
{
}

void HttpSubsystem::initialize(Application& app)
{
    Poco::Net::initializeSSL();

    if (app.config().hasProperty("http.port"))
    {
        port_ = app.config().getInt("http.port");
    }

    ServerSocket socket;

    try
    {
        socket = getServerSocket(app);
    }
    catch (Poco::Exception& ex)
    {
        logger_.error("Could not bind to port %i: %s. HTTP server *not* available.", port_, ex.displayText());
        return;
    }

    server_ = std::unique_ptr<HTTPServer>(new HTTPServer(new DefaultRequestHandlerFactory(app.config()), socket, new HTTPServerParams));
    server_->start();

    logger_.information("HTTP server started on port %i.", port_);
}

void HttpSubsystem::uninitialize()
{
    if (server_)
    {
        server_->stop();
        server_.reset(nullptr);
    }

    Poco::Net::uninitializeSSL();
}

const char* HttpSubsystem::name() const
{
    return "HTTP";
}

ServerSocket HttpSubsystem::getServerSocket(Application& app)
{
    if (app.config().getBool("http.ssl.enabled", false))
    {
        return SecureServerSocket(port_);
    }
    else
    {
        return ServerSocket(port_);
    }
}
