#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_SESSIONSETTINGSWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_SESSIONSETTINGSWRAPPER_HPP

#define DUK_PROP(name) \
    static int get##name(void* ctx); \
    static int set##name(void* ctx);

namespace libtorrent
{
    struct session_settings;
}

namespace Hadouken
{
    namespace Scripting
    {
        namespace Modules
        {
            namespace BitTorrent
            {
                class SessionSettingsWrapper
                {
                public:
                    static void initialize(void* ctx, libtorrent::session_settings& settings);

                private:
                    static int finalize(void* ctx);

                    DUK_PROP(UserAgent);
                    DUK_PROP(TrackerCompletionTimeout);
                    DUK_PROP(TrackerReceiveTimeout);
                    DUK_PROP(StopTrackerTimeout);
                    DUK_PROP(TrackerMaximumResponseLength);
                    DUK_PROP(PieceTimeout);
                    DUK_PROP(RequestTimeout);
                    DUK_PROP(RequestQueueTime);
                    DUK_PROP(MaxAllowedInRequestQueue);
                    DUK_PROP(MaxOutRequestQueue);
                    DUK_PROP(WholePiecesThreshold);
                    DUK_PROP(PeerTimeout);
                    DUK_PROP(UrlSeedTimeout);
                    DUK_PROP(UrlSeedPipelineSize);
                };
            }
        }
    }
}

#endif
