#ifndef HADOUKEN_BITTORRENT_PROXYSETTINGS_HPP
#define HADOUKEN_BITTORRENT_PROXYSETTINGS_HPP

#include <libtorrent/session_settings.hpp>

namespace Hadouken
{
    namespace BitTorrent
    {
        class Session;

        struct ProxySettings
        {
            friend class Session;

            enum ProxyType
            {
                None,
                SOCKS4,
                SOCKS5,
                SOCKS5_Password,
                HTTP,
                HTTP_Password,
                I2P_Proxy,
           };

           explicit ProxySettings();
           explicit ProxySettings(const libtorrent::proxy_settings& settings);

           std::string getHost() const;
           void setHost(std::string host);

           std::string getUserName() const;
           void setUserName(std::string userName);

           std::string getPassword() const;
           void setPassword(std::string password);

           uint16_t getPort() const;
           void setPort(uint16_t port);

           ProxyType getType() const;
           void setType(ProxyType type);

        private:
            libtorrent::proxy_settings settings_;
        };
    }
}

#endif
