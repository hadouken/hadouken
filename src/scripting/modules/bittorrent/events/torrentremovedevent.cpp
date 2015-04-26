#include <Hadouken/Scripting/Modules/BitTorrent/Events/TorrentRemovedEvent.hpp>

#include "../../../duktape.h"

using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

TorrentRemovedEvent::TorrentRemovedEvent(std::string infoHash)
    : infoHash_(infoHash)
{
}

void TorrentRemovedEvent::push(duk_context* ctx)
{
    duk_push_string(ctx, infoHash_.c_str());
}

