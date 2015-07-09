#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ERRORCODEWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ERRORCODEWRAPPER_HPP

#include <boost/system/error_code.hpp>

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            namespace bittorrent
            {
                class error_code_wrapper
                {
                public:
                    static void initialize(void* ctx, const boost::system::error_code& ec);
                };
            }
        }
    }
}

#endif
