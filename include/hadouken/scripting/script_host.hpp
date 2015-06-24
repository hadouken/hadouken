#ifndef HADOUKEN_SCRIPTING_SCRIPTHOST_HPP
#define HADOUKEN_SCRIPTING_SCRIPTHOST_HPP

#include <boost/filesystem.hpp>
#include <boost/property_tree/ptree.hpp>
#include <memory>
#include <mutex>
#include <string>
#include <thread>

namespace pt = boost::property_tree;

namespace libtorrent
{
    class alert;
}

namespace hadouken
{
    class application;

    namespace scripting
    {
        class script_host
        {
        public:
            script_host(hadouken::application& app);
            ~script_host();

            void emit(std::string name, libtorrent::alert* alert);

            std::string rpc(std::string request);

            void load(boost::filesystem::path script_path);

            void unload();

            void define_global(std::string variable, std::string value);

            bool is_authenticated(std::string authHeader);

        protected:
            void tick();

        private:
            typedef void duk_context;
            
            static int require_native(duk_context* ctx);

            hadouken::application& app_;
            bool is_running_;
            duk_context* ctx_;
            std::mutex ctx_mutex_;
            std::thread ticker_;
            std::thread reader_;
        };
    }
}

#endif
