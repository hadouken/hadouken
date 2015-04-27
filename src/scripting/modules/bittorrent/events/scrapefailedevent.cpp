#include <Hadouken/Scripting/Modules/BitTorrent/Events/ScrapeFailedEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

ScrapeFailedEvent::ScrapeFailedEvent(std::shared_ptr<TorrentHandle> handle, std::string url, std::string message)
    : TrackerEvent(handle, url),
    message_(message)
{
}

void ScrapeFailedEvent::push(duk_context* ctx)
{
    duk_idx_t idx = duk_push_object(ctx);
    TrackerEvent::push(ctx, idx);
    
    duk_push_string(ctx, message_.c_str());
    duk_put_prop_string(ctx, idx, "message");
}
