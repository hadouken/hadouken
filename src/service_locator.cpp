#include <hadouken/service_locator.hpp>

#include <string>

using namespace hadouken;

service_locator::service_locator()
{
    services_ = new std::map<std::string, void*>();
}

service_locator::~service_locator()
{
    delete services_;
}

void service_locator::add_service(const std::string& name, void* service)
{
    services_->insert(std::make_pair(name, service));
}
