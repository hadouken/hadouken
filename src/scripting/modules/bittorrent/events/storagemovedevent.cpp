#include <Hadouken/Scripting/Modules/BitTorrent/Events/StorageMovedEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

StorageMovedEvent::StorageMovedEvent(std::shared_ptr<TorrentHandle> handle, std::string path)
    : TorrentEvent(handle)
{
    path_ = path;
}

void StorageMovedEvent::push(duk_context* ctx)
{
    duk_idx_t idx = duk_push_object(ctx);
    
    TorrentEvent::push(ctx);
    duk_put_prop_string(ctx, idx, "torrent");

    duk_push_string(ctx, path_.c_str());
    duk_put_prop_string(ctx, idx, "path");
}
