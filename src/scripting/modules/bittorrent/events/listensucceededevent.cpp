#include <Hadouken/Scripting/Modules/BitTorrent/Events/ListenSucceededEvent.hpp>

#include "../../../duktape.h"

using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

ListenSucceededEvent::ListenSucceededEvent(std::string address, int port, int type)
    : address_(address)
{
    port_ = port;
    type_ = type;
}

void ListenSucceededEvent::push(duk_context* ctx)
{
    duk_idx_t idx = duk_push_object(ctx);
    duk_idx_t ipIdx = duk_push_object(ctx);

    duk_push_string(ctx, address_.c_str());
    duk_put_prop_string(ctx, ipIdx, "address");

    duk_push_int(ctx, port_);
    duk_put_prop_string(ctx, ipIdx, "port");

    duk_put_prop_string(ctx, idx, "ip");

    duk_push_int(ctx, type_);
    duk_put_prop_string(ctx, idx, "type");
}
