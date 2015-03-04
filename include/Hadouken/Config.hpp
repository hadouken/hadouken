#ifndef HADOUKEN_CONFIG_HPP
#define HADOUKEN_CONFIG_HPP

#ifdef WIN32
    #define HDKN_EXPORT __declspec(dllexport)
#else
    #define HDKN_EXPORT
#endif

#endif
