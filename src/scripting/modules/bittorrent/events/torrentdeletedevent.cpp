#include <Hadouken/Scripting/Modules/BitTorrent/Events/TorrentDeletedEvent.hpp>

#include "../../../duktape.h"

using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

TorrentDeletedEvent::TorrentDeletedEvent(std::string infoHash)
    : infoHash_(infoHash)
{
}

void TorrentDeletedEvent::push(duk_context* ctx)
{
    duk_push_string(ctx, infoHash_.c_str());
}
