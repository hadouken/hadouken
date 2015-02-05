#ifndef HDKN_SERVICE_LOCATOR_HPP
#define HDKN_SERVICE_LOCATOR_HPP

#ifdef WIN32
    #define HDKN_API __declspec(dllexport)
#else
    #define HDKN_API
#endif

#include <map>
#include <string>

namespace hadouken
{
    class HDKN_API service_locator
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
