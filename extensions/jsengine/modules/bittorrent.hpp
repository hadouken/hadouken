#ifndef JSENGINE_BITTORRENT_HPP
#define JSENGINE_BITTORRENT_HPP

#include "../duktape.h"

#include <string>

namespace Hadouken
{
    namespace BitTorrent
    {
        class Session;
        struct TorrentHandle;
        struct TorrentStatus;
    }
}

namespace JsEngine
{
    namespace Modules
    {
        class BitTorrent
        {
        public:
            static duk_ret_t init(duk_context* ctx);

        private:
            static duk_ret_t handleFinalizer(duk_context* ctx);
            static duk_ret_t handleGetInfoHash(duk_context* ctx);
            static duk_ret_t handleGetQueuePosition(duk_context* ctx);
            static duk_ret_t handleGetStatus(duk_context* ctx);
            static duk_ret_t handleMove(duk_context* ctx);
            static duk_ret_t handlePause(duk_context* ctx);
            static duk_ret_t handleResume(duk_context* ctx);

            static duk_ret_t sessionAddTorrentFile(duk_context* ctx);
            static duk_ret_t sessionGetTorrents(duk_context* ctx);

            static duk_ret_t statusFinalizer(duk_context* ctx);
            static duk_ret_t statusGetActiveTime(duk_context* ctx);
            static duk_ret_t statusGetDownloadRate(duk_context* ctx);
            static duk_ret_t statusGetError(duk_context* ctx);
            static duk_ret_t statusGetName(duk_context* ctx);
            static duk_ret_t statusGetProgress(duk_context* ctx);
            static duk_ret_t statusGetSavePath(duk_context* ctx);
            static duk_ret_t statusGetState(duk_context* ctx);
            static duk_ret_t statusGetUploadRate(duk_context* ctx);

            static Hadouken::BitTorrent::Session* getSessionFromThis(duk_context* ctx);
            static Hadouken::BitTorrent::TorrentHandle* getTorrentHandleFromThis(duk_context* ctx);
            static Hadouken::BitTorrent::TorrentStatus* getTorrentStatusFromThis(duk_context* ctx);
            static void setTorrentHandleObject(duk_context* ctx, Hadouken::BitTorrent::TorrentHandle& handle);
            static void setTorrentStatusObject(duk_context* ctx, Hadouken::BitTorrent::TorrentStatus& status);

            static const duk_function_list_entry handle_functions_[];
            static const duk_function_list_entry session_functions_[];
            static const std::string session_name_;
        };
    };
}

#endif
