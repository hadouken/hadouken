#pragma once

#include <boost/algorithm/string.hpp>
#include <string>

namespace hadouken
{
namespace http
{
    static std::string_view mime_type(std::string_view const& path)
    {
        auto const ext = [&path]
        {
            auto const pos = path.rfind(".");
            if(pos == std::string_view::npos)
                return std::string_view{};
            return path.substr(pos);
        }();

        if(boost::iequals(ext, ".htm"))  return "text/html";
        if(boost::iequals(ext, ".html")) return "text/html";
        if(boost::iequals(ext, ".php"))  return "text/html";
        if(boost::iequals(ext, ".css"))  return "text/css";
        if(boost::iequals(ext, ".txt"))  return "text/plain";
        if(boost::iequals(ext, ".js"))   return "application/javascript";
        if(boost::iequals(ext, ".json")) return "application/json";
        if(boost::iequals(ext, ".xml"))  return "application/xml";
        if(boost::iequals(ext, ".swf"))  return "application/x-shockwave-flash";
        if(boost::iequals(ext, ".flv"))  return "video/x-flv";
        if(boost::iequals(ext, ".png"))  return "image/png";
        if(boost::iequals(ext, ".jpe"))  return "image/jpeg";
        if(boost::iequals(ext, ".jpeg")) return "image/jpeg";
        if(boost::iequals(ext, ".jpg"))  return "image/jpeg";
        if(boost::iequals(ext, ".gif"))  return "image/gif";
        if(boost::iequals(ext, ".bmp"))  return "image/bmp";
        if(boost::iequals(ext, ".ico"))  return "image/vnd.microsoft.icon";
        if(boost::iequals(ext, ".tiff")) return "image/tiff";
        if(boost::iequals(ext, ".tif"))  return "image/tiff";
        if(boost::iequals(ext, ".svg"))  return "image/svg+xml";
        if(boost::iequals(ext, ".svgz")) return "image/svg+xml";
        return "application/text";
    }
}
}
