#ifndef HADOUKEN_HTTP_GUIREQUESTHANDLER_HPP
#define HADOUKEN_HTTP_GUIREQUESTHANDLER_HPP

#include <boost/filesystem.hpp>
#include <boost/property_tree/ptree.hpp>
#include <hadouken/http/request_handler.hpp>

namespace hadouken
{
    namespace http
    {
        class gui_request_handler : public request_handler
        {
        public:
            gui_request_handler(const boost::property_tree::ptree& config);

            void execute(std::string virtual_path,
                         http_server_t::request const &request,
                         http_server_t::connection_ptr connection);

        protected:
            std::string get_content_type(std::string extension);
            
            void load_cache();

        private:
            boost::filesystem::path gui_path_;
            boost::property_tree::ptree config_;
            std::map<std::string, std::string> cache_;
            bool use_cache_;
        };
    }
}

#endif
