#ifndef HDKN_PLUGIN_HPP
#define HDKN_PLUGIN_HPP

#define HADOUKEN_PLUGIN(ClassName) \
    extern "C" __declspec(dllexport) ClassName* hdkn_create_##ClassName(void) { return new ClassName(); } \
    extern "C" __declspec(dllexport) void hdkn_destroy_##ClassName(ClassName* instance) { delete instance; }


namespace hadouken
{
    class plugin
    {
    public:
        virtual void load() = 0;
    };
}

#endif
