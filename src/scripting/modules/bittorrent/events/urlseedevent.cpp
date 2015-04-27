#include <Hadouken/Scripting/Modules/BitTorrent/Events/UrlSeedEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

UrlSeedEvent::UrlSeedEvent(std::shared_ptr<TorrentHandle> handle, std::string url, std::string message)
    : TorrentEvent(handle),
    url_(url),
    message_(message)
{
}

void UrlSeedEvent::push(duk_context* ctx)
{
    duk_idx_t idx = duk_push_object(ctx);
    
    TorrentEvent::push(ctx);
    duk_put_prop_string(ctx, idx, "torrent");

    duk_push_string(ctx, url_.c_str());
    duk_put_prop_string(ctx, idx, "url");

    duk_push_string(ctx, message_.c_str());
    duk_put_prop_string(ctx, idx, "message");
}
