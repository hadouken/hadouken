#ifndef HDKN_CONFIG_HPP
#define HDKN_CONFIG_HPP

// Shared defines and macros.

#ifdef WIN32
    #define HDKN_API __declspec(dllexport)
#else
    #define HDKN_API
#endif

#endif