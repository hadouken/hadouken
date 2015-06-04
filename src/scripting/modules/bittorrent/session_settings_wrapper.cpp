#include <hadouken/scripting/modules/bittorrent/session_settings_wrapper.hpp>

#include <libtorrent/session_settings.hpp>

#include "../common.hpp"
#include "../../duktape.h"

#define DUK_STRING_PROP(prop) \
    duk_ret_t session_settings_wrapper::get_##prop(duk_context* ctx) \
        { \
        libtorrent::session_settings* ss = common::get_pointer<libtorrent::session_settings>(ctx); \
        duk_push_string(ctx, ss->##prop.c_str()); \
        return 1; \
        } \
    \
    duk_ret_t session_settings_wrapper::set_##prop(duk_context* ctx) \
        { \
        libtorrent::session_settings* ss = common::get_pointer<libtorrent::session_settings>(ctx); \
        ss->##prop = duk_require_string(ctx, 0); \
        return 0; \
        }

#define DUK_INT_PROP(prop) \
    duk_ret_t session_settings_wrapper::get_##prop(duk_context* ctx) \
        { \
        libtorrent::session_settings* ss = common::get_pointer<libtorrent::session_settings>(ctx); \
        duk_push_int(ctx, ss->##prop); \
        return 1; \
        } \
    \
    duk_ret_t session_settings_wrapper::set_##prop(duk_context* ctx) \
        { \
        libtorrent::session_settings* ss = common::get_pointer<libtorrent::session_settings>(ctx); \
        ss->##prop = duk_require_int(ctx, 0); \
        return 0; \
        }

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

void session_settings_wrapper::initialize(duk_context* ctx, libtorrent::session_settings& settings)
{
    duk_idx_t idx = duk_push_object(ctx);
    common::set_pointer<libtorrent::session_settings>(ctx, idx, new libtorrent::session_settings(settings));

    DUK_READWRITE_PROPERTY(ctx, idx, userAgent, user_agent);
    DUK_READWRITE_PROPERTY(ctx, idx, trackerCompletionTimeout, tracker_completion_timeout);
    DUK_READWRITE_PROPERTY(ctx, idx, trackerReceiveTimeout, tracker_receive_timeout);
    DUK_READWRITE_PROPERTY(ctx, idx, stopTrackerTimeout, stop_tracker_timeout);
    DUK_READWRITE_PROPERTY(ctx, idx, trackerMaximumResponseLength, tracker_maximum_response_length);
    DUK_READWRITE_PROPERTY(ctx, idx, pieceTimeout, piece_timeout);
    DUK_READWRITE_PROPERTY(ctx, idx, requestTimeout, request_timeout);
    DUK_READWRITE_PROPERTY(ctx, idx, requestQueueTime, request_queue_time);
    DUK_READWRITE_PROPERTY(ctx, idx, maxAllowedInRequestQueue, max_allowed_in_request_queue);
    DUK_READWRITE_PROPERTY(ctx, idx, maxOutRequestQueue, max_out_request_queue);
    DUK_READWRITE_PROPERTY(ctx, idx, wholePiecesThreshold, whole_pieces_threshold);
    DUK_READWRITE_PROPERTY(ctx, idx, peerTimeout, peer_timeout);
    DUK_READWRITE_PROPERTY(ctx, idx, urlSeedTimeout, urlseed_timeout);
    DUK_READWRITE_PROPERTY(ctx, idx, urlSeedPipelineSize, urlseed_pipeline_size);

    // Set finalizer
    duk_push_c_function(ctx, finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t session_settings_wrapper::finalize(duk_context* ctx)
{
    common::finalize<libtorrent::session_settings>(ctx);
    return 0;
}

DUK_STRING_PROP(user_agent)
DUK_INT_PROP(tracker_completion_timeout)
DUK_INT_PROP(tracker_receive_timeout)
DUK_INT_PROP(stop_tracker_timeout)
DUK_INT_PROP(tracker_maximum_response_length)
DUK_INT_PROP(piece_timeout)
DUK_INT_PROP(request_timeout)
DUK_INT_PROP(request_queue_time)
DUK_INT_PROP(max_allowed_in_request_queue)
DUK_INT_PROP(max_out_request_queue)
DUK_INT_PROP(whole_pieces_threshold)
DUK_INT_PROP(peer_timeout)
DUK_INT_PROP(urlseed_timeout)
DUK_INT_PROP(urlseed_pipeline_size)
