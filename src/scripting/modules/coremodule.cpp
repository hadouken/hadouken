#include <Hadouken/Scripting/Modules/CoreModule.hpp>

#include <Hadouken/Version.hpp>
#include "../duktape.h"

using namespace Hadouken::Scripting::Modules;

duk_ret_t CoreModule::initialize(duk_context* ctx)
{
    duk_push_string(ctx, Hadouken::Version::GIT_BRANCH().c_str());
    duk_put_prop_string(ctx, 0 /* exports */, "GIT_BRANCH");

    duk_push_string(ctx, Hadouken::Version::GIT_COMMIT_HASH().c_str());
    duk_put_prop_string(ctx, 0 /* exports */, "GIT_COMMIT_HASH");

    duk_push_string(ctx, Hadouken::Version::VERSION().c_str());
    duk_put_prop_string(ctx, 0 /* exports */, "HADOUKEN_VERSION");

    return 0;
}
