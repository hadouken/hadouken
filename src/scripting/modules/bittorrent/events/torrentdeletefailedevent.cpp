#include <Hadouken/Scripting/Modules/BitTorrent/Events/TorrentDeleteFailedEvent.hpp>

#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

TorrentDeleteFailedEvent::TorrentDeleteFailedEvent(std::string infoHash, Error error)
    : infoHash_(infoHash),
    error_(error)
{
}

void TorrentDeleteFailedEvent::push(duk_context* ctx)
{
    duk_idx_t idx = duk_push_object(ctx);

    duk_push_string(ctx, infoHash_.c_str());
    duk_put_prop_string(ctx, idx, "infoHash");

    // ---- error
    duk_idx_t errIdx = duk_push_object(ctx);
    
    duk_push_int(ctx, error_.code);
    duk_put_prop_string(ctx, errIdx, "code");

    duk_push_string(ctx, error_.message.c_str());
    duk_put_prop_string(ctx, errIdx, "message");

    duk_put_prop_string(ctx, idx, "error");
}
