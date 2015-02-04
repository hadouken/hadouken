#ifndef HDKN_SERVICE_LOCATOR_HPP
#define HDKN_SERVICE_LOCATOR_HPP

#include <map>
#include <string>

namespace hadouken
{
    class __declspec(dllexport) service_locator
    {
    public:
        service_locator();
        ~service_locator();

        template<typename T>
        T request(const std::string id)
        {
            return static_cast<T>(services_->at(id));
        }

        void add_service(const std::string& name, void* service);

    private:
        std::map<std::string, void*>* services_;
    };
}

#endif