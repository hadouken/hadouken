#include <Hadouken/Scripting/Modules/BitTorrent/Events/ExternalAddressEvent.hpp>

#include "../../../duktape.h"

using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

ExternalAddressEvent::ExternalAddressEvent(std::string address)
    : address_(address)
{
}

void ExternalAddressEvent::push(duk_context* ctx)
{
    duk_push_string(ctx, address_.c_str());
}
