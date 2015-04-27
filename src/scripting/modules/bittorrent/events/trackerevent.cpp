#include <Hadouken/Scripting/Modules/BitTorrent/Events/TrackerEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>

#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

TrackerEvent::TrackerEvent(std::shared_ptr<TorrentHandle> handle, std::string url)
    : TorrentEvent(handle),
    url_(url)
{
}

void TrackerEvent::push(duk_context* ctx, duk_idx_t idx)
{
    TorrentEvent::push(ctx);
    duk_put_prop_string(ctx, idx, "torrent");

    duk_push_string(ctx, url_.c_str());
    duk_put_prop_string(ctx, idx, "url");
}
