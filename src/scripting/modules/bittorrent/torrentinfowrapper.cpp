#include <Hadouken/Scripting/Modules/BitTorrent/TorrentInfoWrapper.hpp>

#include <Hadouken/BitTorrent/FileEntry.hpp>
#include <Hadouken/BitTorrent/FileStorage.hpp>
#include <Hadouken/BitTorrent/TorrentInfo.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/TorrentHandleWrapper.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;

void TorrentInfoWrapper::initialize(duk_context* ctx, TorrentHandle& handle, std::unique_ptr<TorrentInfo> info)
{
    duk_function_list_entry functions[] =
    {
        { "getFiles", TorrentInfoWrapper::getFiles, 0 },
        { NULL, NULL, 0 }
    };

    duk_idx_t infoIndex = duk_push_object(ctx);
    duk_put_function_list(ctx, infoIndex, functions);

    // Set internal pointers
    Common::setPointer<TorrentHandle>(ctx, infoIndex, new TorrentHandle(handle));
    Common::setPointer<TorrentInfo>(ctx, infoIndex, info.release());

    DUK_READONLY_PROPERTY(ctx, infoIndex, totalSize, TorrentInfoWrapper::getTotalSize);

    duk_push_c_function(ctx, TorrentInfoWrapper::finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t TorrentInfoWrapper::finalize(duk_context* ctx)
{
    Common::finalize<TorrentHandle>(ctx);
    Common::finalize<TorrentInfo>(ctx);

    return 0;
}

duk_ret_t TorrentInfoWrapper::getFiles(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    TorrentInfo* info = Common::getPointer<TorrentInfo>(ctx);

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    FileStorage fileStorage = info->getFiles();
    std::vector<int64_t> progress;
    handle->getFileProgress(progress);

    for (int i = 0; i < fileStorage.getNumFiles(); i++)
    {
        FileEntry entry = fileStorage.getEntryAt(i);

        duk_idx_t entryIndex = duk_push_object(ctx);
        duk_push_string(ctx, entry.getPath().c_str());
        duk_put_prop_string(ctx, entryIndex, "path");

        duk_push_number(ctx, static_cast<duk_double_t>(progress[i]));
        duk_put_prop_string(ctx, entryIndex, "progress");

        duk_push_number(ctx, static_cast<duk_double_t>(entry.getSize()));
        duk_put_prop_string(ctx, entryIndex, "size");

        // Put entry object
        duk_put_prop_index(ctx, arrayIndex, i);
    }

    return 1;
}

duk_ret_t TorrentInfoWrapper::getTotalSize(duk_context* ctx)
{
    TorrentInfo* info = Common::getPointer<TorrentInfo>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(info->getTotalSize()));
    return 1;
}
