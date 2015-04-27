#ifndef HADOUKEN_BITTORRENT_ERROR_HPP
#define HADOUKEN_BITTORRENT_ERROR_HPP

#include <string>

namespace Hadouken
{
    namespace BitTorrent
    {
        struct Error
        {
            Error(int errorCode, std::string message)
                : code(errorCode),
                message(message)
            {
            }

            const int code;
            const std::string message;
        };
    }
}

#endif
