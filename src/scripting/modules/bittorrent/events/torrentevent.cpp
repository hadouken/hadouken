#include <Hadouken/Scripting/Modules/BitTorrent/Events/TorrentEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/TorrentHandleWrapper.hpp>

#include "../../common.hpp"
#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

TorrentEvent::TorrentEvent(std::shared_ptr<TorrentHandle> handle)
    : handle_(handle)
{
}

void TorrentEvent::push(duk_context* ctx)
{
    TorrentHandleWrapper::initialize(ctx, handle_);
}
