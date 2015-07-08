#include <hadouken/scripting/modules/bittorrent/error_code_wrapper.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

void error_code_wrapper::initialize(duk_context* ctx, const boost::system::error_code& error)
{
    duk_idx_t idx = duk_push_object(ctx);

    duk_push_string(ctx, error.message().c_str());
    duk_put_prop_string(ctx, idx, "message");

    duk_push_int(ctx, error.value());
    duk_put_prop_string(ctx, idx, "value");
}
