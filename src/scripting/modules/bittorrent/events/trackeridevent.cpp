#include <Hadouken/Scripting/Modules/BitTorrent/Events/TrackerIdEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

TrackerIdEvent::TrackerIdEvent(std::shared_ptr<TorrentHandle> handle, std::string url, std::string trackerId)
    : TrackerEvent(handle, url),
    trackerId_(trackerId)
{
}

void TrackerIdEvent::push(duk_context* ctx)
{
    duk_idx_t idx = duk_push_object(ctx);
    TrackerEvent::push(ctx, idx);
    
    duk_push_string(ctx, trackerId_.c_str());
    duk_put_prop_string(ctx, idx, "id");
}
