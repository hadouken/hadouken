#include <Hadouken/BitTorrent/ProxySettings.hpp>

#include <libtorrent/session_settings.hpp>

using namespace Hadouken::BitTorrent;

ProxySettings::ProxySettings()
{
}

ProxySettings::ProxySettings(const libtorrent::proxy_settings& settings)
    : settings_(settings)
{
}

std::string ProxySettings::getHost() const
{
    return settings_.hostname;
}

void ProxySettings::setHost(std::string host)
{
    settings_.hostname = host;
}

std::string ProxySettings::getUserName() const
{
    return settings_.username;
}

void ProxySettings::setUserName(std::string userName)
{
    settings_.username = userName;
}

std::string ProxySettings::getPassword() const
{
    return settings_.password;
}

void ProxySettings::setPassword(std::string password)
{
    settings_.password = password;
}

uint16_t ProxySettings::getPort() const
{
    return settings_.port;
}

void ProxySettings::setPort(uint16_t port)
{
    settings_.port = port;
}

ProxySettings::ProxyType ProxySettings::getType() const
{
    return (ProxySettings::ProxyType)(int)settings_.type;
}

void ProxySettings::setType(ProxySettings::ProxyType type)
{
    settings_.type = (libtorrent::proxy_settings::proxy_type)(int)type;
}
