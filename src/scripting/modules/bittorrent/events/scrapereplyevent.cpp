#include <Hadouken/Scripting/Modules/BitTorrent/Events/ScrapeReplyEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

ScrapeReplyEvent::ScrapeReplyEvent(std::shared_ptr<TorrentHandle> handle, std::string url, int complete, int incomplete)
    : TrackerEvent(handle, url)
{
    complete_ = complete;
    incomplete_ = incomplete;
}

void ScrapeReplyEvent::push(duk_context* ctx)
{
    duk_idx_t idx = duk_push_object(ctx);
    TrackerEvent::push(ctx, idx);
    
    duk_push_int(ctx, complete_);
    duk_put_prop_string(ctx, idx, "complete");

    duk_push_int(ctx, incomplete_);
    duk_put_prop_string(ctx, idx, "incomplete");
}
