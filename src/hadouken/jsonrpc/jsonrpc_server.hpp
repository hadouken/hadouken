#pragma once

#include <map>
#include <memory>

namespace hadouken
{
namespace jsonrpc
{
    class jsonrpc_method;

    class jsonrpc_server
    {
    public:
        jsonrpc_server();

        void add_method(std::string const& name, std::shared_ptr<jsonrpc_method> method);
        std::string execute(std::string const& body);

    private:
        std::map<std::string, std::shared_ptr<jsonrpc_method>> m_methods;
    };
}
}
