#include <Hadouken/Scripting/Modules/BitTorrent/Events/FileRenamedEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

FileRenamedEvent::FileRenamedEvent(std::shared_ptr<TorrentHandle> handle, int index, std::string name)
    : TorrentEvent(handle)
{
    index_ = index;
    name_ = name;
}

void FileRenamedEvent::push(duk_context* ctx)
{
    duk_idx_t idx = duk_push_object(ctx);
    
    TorrentEvent::push(ctx);
    duk_put_prop_string(ctx, idx, "torrent");

    duk_push_int(ctx, index_);
    duk_put_prop_string(ctx, idx, "index");

    duk_push_string(ctx, name_.c_str());
    duk_put_prop_string(ctx, idx, "name");
}
