#include "jsonrpc_server.hpp"

#include "jsonrpc_method.hpp"

using hadouken::jsonrpc::jsonrpc_server;

jsonrpc_server::jsonrpc_server()
{
}

void jsonrpc_server::add_method(std::string const& name, std::shared_ptr<hadouken::jsonrpc::jsonrpc_method> method)
{
    m_methods.insert({ name, method });
}

std::string jsonrpc_server::execute(std::string const& body)
{
    json data = json::parse(body);

    auto it = m_methods.find(data["method"]);

    if (it != m_methods.end())
    {
        json res;
        res["jsonrpc"] = "2.0";
        res["id"] = data["id"];
        res["result"] = it->second->execute(data["params"]);

        return res.dump();
    }

    return "{\"hej\": 2}";
}
