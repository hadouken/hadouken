#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENTMODULE_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENTMODULE_HPP

#include <memory>

namespace Hadouken
{
    namespace Scripting
    {
        namespace Modules
        {
            class BitTorrentModule
            {
            public:
                static int initialize(void* ctx);

            private:
                static int getSession(void* ctx);
            };
        }
    }
}

#endif