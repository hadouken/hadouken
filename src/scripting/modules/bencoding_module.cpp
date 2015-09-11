#include <hadouken/scripting/modules/bencoding_module.hpp>

#include <hadouken/scripting/modules/bittorrent/lazy_entry_wrapper.hpp>
#include <libtorrent/bencode.hpp>
#include <libtorrent/entry.hpp>
#include <libtorrent/lazy_entry.hpp>

#include "common.hpp"
#include "../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

duk_ret_t bencoding_module::initialize(duk_context* ctx)
{
    duk_function_list_entry functions[] =
    {
        { "decode", decode, 1 },
        { "encode", encode, 1 },
        { NULL,     NULL,   0 }
    };

    duk_put_function_list(ctx, 0, functions);
    return 0;
}

duk_ret_t bencoding_module::decode(duk_context* ctx)
{
    /*duk_size_t size;
    const char* buffer = static_cast<const char*>(duk_require_buffer(ctx, 0, &size));

    libtorrent::error_code ec;
    libtorrent::lazy_entry entry;
    libtorrent::lazy_bdecode(buffer, buffer + size, entry, ec);

    lazy_entry_wrapper::initialize(ctx, entry);*/
    return 1;
}

duk_ret_t bencoding_module::encode(duk_context* ctx)
{
    libtorrent::entry* entry = common::get_pointer<libtorrent::entry>(ctx, 0);

    std::vector<char> data;
    libtorrent::bencode(std::back_inserter(data), *entry);

    void* buffer = duk_push_buffer(ctx, data.size(), false);
    std::copy(data.begin(), data.end(), static_cast<char*>(buffer));

    return 1;
}
