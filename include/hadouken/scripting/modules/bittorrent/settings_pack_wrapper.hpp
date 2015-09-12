#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_SETTINGSPACKWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_SETTINGSPACKWRAPPER_HPP

namespace libtorrent
{
    struct settings_pack;
}

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            namespace bittorrent
            {
                class settings_pack_wrapper
                {
                public:
                    static void initialize(void* ctx, libtorrent::settings_pack& settings);

                private:
                    static int finalize(void* ctx);

                    static int clear(void* ctx);
                    static int get_bool(void* ctx);
                    static int get_int(void* ctx);
                    static int get_str(void* ctx);
                    static int has_val(void* ctx);
                    static int set_bool(void* ctx);
                    static int set_int(void* ctx);
                    static int set_str(void* ctx);
                };
            }
        }
    }
}

#endif