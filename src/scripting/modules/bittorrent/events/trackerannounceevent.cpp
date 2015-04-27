#include <Hadouken/Scripting/Modules/BitTorrent/Events/TrackerAnnounceEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

TrackerAnnounceEvent::TrackerAnnounceEvent(std::shared_ptr<TorrentHandle> handle, std::string url, int event)
    : TrackerEvent(handle, url)
{
    event_ = event;
}

void TrackerAnnounceEvent::push(duk_context* ctx)
{
    duk_idx_t idx = duk_push_object(ctx);
    TrackerEvent::push(ctx, idx);
    
    duk_push_int(ctx, event_);
    duk_put_prop_string(ctx, idx, "event");
}
