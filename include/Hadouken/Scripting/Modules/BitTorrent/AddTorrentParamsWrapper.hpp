#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ADDTORRENTPARAMSWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ADDTORRENTPARAMSWRAPPER_HPP

namespace Hadouken
{
    namespace Scripting
    {
        namespace Modules
        {
            namespace BitTorrent
            {
                class AddTorrentParamsWrapper
                {
                public:
                    static int construct(void* ctx);

                private:
                    static int destruct(void* ctx);

                    static int getFlags(void* ctx);
                    static int getResumeData(void* ctx);
                    static int getSavePath(void* ctx);
                    static int getSparseMode(void* ctx);
                    static int getTorrent(void* ctx);
                    static int getUrl(void* ctx);
                    static int setFlags(void* ctx);
                    static int setResumeData(void* ctx);
                    static int setSavePath(void* ctx);
                    static int setSparseMode(void* ctx);
                    static int setTorrent(void* ctx);
                    static int setUrl(void* ctx);
                };
            }
        }
    }
}

#endif
