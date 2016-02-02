#include <hadouken/scripting/modules/bittorrent/settings_pack_wrapper.hpp>

#include <libtorrent/settings_pack.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;
namespace lt = libtorrent;

void settings_pack_wrapper::initialize(duk_context* ctx, lt::settings_pack& settings)
{
    duk_function_list_entry functions[] =
    {
        { "clear", clear, 0},
        { "getBool", get_bool, 1},
        { "getInt", get_int, 1},
        { "getStr", get_str, 1},
        { "hasVal", has_val, 1},
        { "setBool", set_bool, 2},
        { "setInt", set_int, 2},
        { "setStr", set_str, 2},
        { NULL, NULL, 0 }
    };

    duk_idx_t idx = duk_push_object(ctx);
    duk_put_function_list(ctx, idx, functions);

    common::set_pointer<lt::settings_pack>(ctx, idx, new lt::settings_pack(settings));

    duk_push_c_function(ctx, finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t settings_pack_wrapper::finalize(duk_context* ctx)
{
    common::finalize<lt::settings_pack>(ctx);
    return 0;
}

duk_ret_t settings_pack_wrapper::clear(duk_context* ctx)
{
    lt::settings_pack* sp = common::get_pointer<lt::settings_pack>(ctx);
    sp->clear();
    return 0;
}

duk_ret_t settings_pack_wrapper::get_bool(duk_context* ctx)
{
    lt::settings_pack* sp = common::get_pointer<lt::settings_pack>(ctx);
    std::string name = duk_require_string(ctx, 0);
    int n = lt::setting_by_name(name);

    duk_push_boolean(ctx, sp->get_bool(n));

    return 1;
}

duk_ret_t settings_pack_wrapper::get_int(duk_context* ctx)
{
    lt::settings_pack* sp = common::get_pointer<lt::settings_pack>(ctx);
    std::string name = duk_require_string(ctx, 0);
    int n = lt::setting_by_name(name);

    duk_push_int(ctx, sp->get_int(n));

    return 1;
}

duk_ret_t settings_pack_wrapper::get_str(duk_context* ctx)
{
    lt::settings_pack* sp = common::get_pointer<lt::settings_pack>(ctx);
    std::string name = duk_require_string(ctx, 0);
    int n = lt::setting_by_name(name);

    duk_push_string(ctx, sp->get_str(n).c_str());

    return 1;
}

duk_ret_t settings_pack_wrapper::has_val(duk_context* ctx)
{
    lt::settings_pack* sp = common::get_pointer<lt::settings_pack>(ctx);
    std::string name = duk_require_string(ctx, 0);
    int n = lt::setting_by_name(name);

    duk_push_boolean(ctx, sp->has_val(n));

    return 1;
}

duk_ret_t settings_pack_wrapper::set_bool(duk_context* ctx)
{
    lt::settings_pack* sp = common::get_pointer<lt::settings_pack>(ctx);
    std::string name = duk_require_string(ctx, 0);
    int n = lt::setting_by_name(name);
    bool val = duk_require_boolean(ctx, 1);

    sp->set_bool(n, val);

    return 0;
}

duk_ret_t settings_pack_wrapper::set_int(duk_context* ctx)
{
    lt::settings_pack* sp = common::get_pointer<lt::settings_pack>(ctx);
    std::string name = duk_require_string(ctx, 0);
    int n = lt::setting_by_name(name);
    int val = duk_require_int(ctx, 1);

    sp->set_int(n, val);

    return 0;
}

duk_ret_t settings_pack_wrapper::set_str(duk_context* ctx)
{
    lt::settings_pack* sp = common::get_pointer<lt::settings_pack>(ctx);
    std::string name = duk_require_string(ctx, 0);
    int n = lt::setting_by_name(name);
    std::string val = duk_require_string(ctx, 1);

    sp->set_str(n, val);

    return 0;
}
