#include <hadouken/scripting/modules/core_module.hpp>

#include <hadouken/version.hpp>
#include "../duktape.h"

using namespace hadouken::scripting::modules;

duk_ret_t core_module::initialize(duk_context* ctx)
{
    duk_push_string(ctx, hadouken::version::GIT_BRANCH().c_str());
    duk_put_prop_string(ctx, 0 /* exports */, "GIT_BRANCH");

    duk_push_string(ctx, hadouken::version::GIT_COMMIT_HASH().c_str());
    duk_put_prop_string(ctx, 0 /* exports */, "GIT_COMMIT_HASH");

    duk_push_string(ctx, hadouken::version::VERSION().c_str());
    duk_put_prop_string(ctx, 0 /* exports */, "HADOUKEN_VERSION");

    return 0;
}
