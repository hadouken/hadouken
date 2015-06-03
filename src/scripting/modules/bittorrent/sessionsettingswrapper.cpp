#include <Hadouken/Scripting/Modules/BitTorrent/SessionSettingsWrapper.hpp>

#include <libtorrent/session_settings.hpp>

#include "../common.hpp"
#include "../../duktape.h"

#define DUK_STRING_PROP(name, prop) \
    duk_ret_t SessionSettingsWrapper::get##name(duk_context* ctx) \
        { \
        libtorrent::session_settings* ss = Common::getPointer<libtorrent::session_settings>(ctx); \
        duk_push_string(ctx, ss->##prop.c_str()); \
        return 1; \
        } \
    \
    duk_ret_t SessionSettingsWrapper::set##name(duk_context* ctx) \
        { \
        libtorrent::session_settings* ss = Common::getPointer<libtorrent::session_settings>(ctx); \
        ss->##prop = duk_require_string(ctx, 0); \
        return 0; \
        }

#define DUK_INT_PROP(name, prop) \
    duk_ret_t SessionSettingsWrapper::get##name(duk_context* ctx) \
        { \
        libtorrent::session_settings* ss = Common::getPointer<libtorrent::session_settings>(ctx); \
        duk_push_int(ctx, ss->##prop); \
        return 1; \
        } \
    \
    duk_ret_t SessionSettingsWrapper::set##name(duk_context* ctx) \
        { \
        libtorrent::session_settings* ss = Common::getPointer<libtorrent::session_settings>(ctx); \
        ss->##prop = duk_require_int(ctx, 0); \
        return 0; \
        }

using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;

void SessionSettingsWrapper::initialize(duk_context* ctx, libtorrent::session_settings& settings)
{
    duk_idx_t idx = duk_push_object(ctx);
    Common::setPointer<libtorrent::session_settings>(ctx, idx, new libtorrent::session_settings(settings));

    DUK_READWRITE_PROPERTY(ctx, idx, userAgent, UserAgent);
    DUK_READWRITE_PROPERTY(ctx, idx, trackerCompletionTimeout, TrackerCompletionTimeout);
    DUK_READWRITE_PROPERTY(ctx, idx, trackerReceiveTimeout, TrackerReceiveTimeout);
    DUK_READWRITE_PROPERTY(ctx, idx, stopTrackerTimeout, StopTrackerTimeout);
    DUK_READWRITE_PROPERTY(ctx, idx, trackerMaximumResponseLength, TrackerMaximumResponseLength);
    DUK_READWRITE_PROPERTY(ctx, idx, pieceTimeout, PieceTimeout);
    DUK_READWRITE_PROPERTY(ctx, idx, requestTimeout, RequestTimeout);
    DUK_READWRITE_PROPERTY(ctx, idx, requestQueueTime, RequestQueueTime);
    DUK_READWRITE_PROPERTY(ctx, idx, maxAllowedInRequestQueue, MaxAllowedInRequestQueue);
    DUK_READWRITE_PROPERTY(ctx, idx, maxOutRequestQueue, MaxOutRequestQueue);
    DUK_READWRITE_PROPERTY(ctx, idx, wholePiecesThreshold, WholePiecesThreshold);
    DUK_READWRITE_PROPERTY(ctx, idx, peerTimeout, PeerTimeout);
    DUK_READWRITE_PROPERTY(ctx, idx, urlSeedTimeout, UrlSeedTimeout);
    DUK_READWRITE_PROPERTY(ctx, idx, urlSeedPipelineSize, UrlSeedPipelineSize);

    // Set finalizer
    duk_push_c_function(ctx, finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t SessionSettingsWrapper::finalize(duk_context* ctx)
{
    Common::finalize<libtorrent::session_settings>(ctx);
    return 0;
}

DUK_STRING_PROP(UserAgent, user_agent)
DUK_INT_PROP(TrackerCompletionTimeout, tracker_completion_timeout)
DUK_INT_PROP(TrackerReceiveTimeout, tracker_receive_timeout)
DUK_INT_PROP(StopTrackerTimeout, stop_tracker_timeout)
DUK_INT_PROP(TrackerMaximumResponseLength, tracker_maximum_response_length)
DUK_INT_PROP(PieceTimeout, piece_timeout)
DUK_INT_PROP(RequestTimeout, request_timeout)
DUK_INT_PROP(RequestQueueTime, request_queue_time)
DUK_INT_PROP(MaxAllowedInRequestQueue, max_allowed_in_request_queue)
DUK_INT_PROP(MaxOutRequestQueue, max_out_request_queue)
DUK_INT_PROP(WholePiecesThreshold, whole_pieces_threshold)
DUK_INT_PROP(PeerTimeout, peer_timeout)
DUK_INT_PROP(UrlSeedTimeout, urlseed_timeout)
DUK_INT_PROP(UrlSeedPipelineSize, urlseed_pipeline_size)
