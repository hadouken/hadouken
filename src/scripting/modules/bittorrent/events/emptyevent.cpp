#include <Hadouken/Scripting/Modules/BitTorrent/Events/EmptyEvent.hpp>

#include "../../../duktape.h"

using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

void EmptyEvent::push(duk_context* ctx)
{
    duk_push_undefined(ctx);
}
