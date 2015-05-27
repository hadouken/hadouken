#include <Hadouken/Scripting/Modules/BitTorrent/TorrentCreatorWrapper.hpp>

#include <Hadouken/Scripting/Modules/BitTorrent/EntryWrapper.hpp>
#include <libtorrent/create_torrent.hpp>
#include <libtorrent/entry.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;

duk_ret_t TorrentCreatorWrapper::construct(duk_context* ctx)
{
    duk_push_this(ctx);

    libtorrent::torrent_info* info = Common::getPointer<libtorrent::torrent_info>(ctx, 0);
    libtorrent::create_torrent* creator = new libtorrent::create_torrent(*info);

    Common::setPointer<libtorrent::create_torrent>(ctx, -2, creator);

    duk_function_list_entry functions[] =
    {
        { "generate", generate, 0 },
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, -1, functions);

    duk_push_c_function(ctx, finalize, 1);
    duk_set_finalizer(ctx, -2);

    return 0;
}

duk_ret_t TorrentCreatorWrapper::finalize(duk_context* ctx)
{
    Common::finalize<libtorrent::create_torrent>(ctx);
    return 0;
}

duk_ret_t TorrentCreatorWrapper::generate(duk_context* ctx)
{
    libtorrent::create_torrent* creator = Common::getPointer<libtorrent::create_torrent>(ctx);
    libtorrent::entry entry = creator->generate();

    EntryWrapper::initialize(ctx, entry);
    return 1;
}
