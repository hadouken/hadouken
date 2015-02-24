cmake_minimum_required(VERSION 2.8 FATAL_ERROR)

project("hadouken")

if(NOT BUILD_CONFIGURATION)
    set(BUILD_CONFIGURATION "release")
endif()

message("Build configuration: ${BUILD_CONFIGURATION}")

if(WIN32)
    set(LIBTORRENT torrent.lib)

    add_definitions(
        -DPOCO_NO_UNWINDOWS
        -DWIN32
        -DWIN32_LEAN_AND_MEAN
        -D_CRT_SECURE_NO_WARNINGS
        -D_SCL_SECURE_NO_WARNINGS
        -D_WIN32_WINNT=0x0501
    )

    include_directories(
        ${CMAKE_SOURCE_DIR}/win32/libs/hadouken.libtorrent/win32/include
        ${CMAKE_SOURCE_DIR}/win32/libs/hadouken.openssl/win32/${BUILD_CONFIGURATION}/include
        ${CMAKE_SOURCE_DIR}/win32/libs/hadouken.poco/win32/include
    )

    link_directories(
        ${CMAKE_SOURCE_DIR}/win32/libs/hadouken.libtorrent/win32/lib
        ${CMAKE_SOURCE_DIR}/win32/libs/hadouken.openssl/win32/${BUILD_CONFIGURATION}/lib
        ${CMAKE_SOURCE_DIR}/win32/libs/hadouken.poco/win32/lib
    )
else()
    set(LIBBOOST boost_system)
    set(LIBTORRENT libtorrent-rasterbar.so)
    set(LIBPOCO libPocoFoundation.so libPocoJSON.so libPocoNet.so libPocoUtil.so)
    set(PTHREAD pthread)

    add_compile_options(-std=c++0x)
endif()

add_definitions(
    -DBOOST_ASIO_SEPARATE_COMPILATION
    -DBOOST_ALL_DYN_LINK
    -DTORRENT_NO_DEPRECATE
)

include_directories(
    ${CMAKE_SOURCE_DIR}/include
)

add_executable(
    hadouken
    src/main.cpp
    src/bittorrent/fileentry.cpp
    src/bittorrent/filestorage.cpp
    src/bittorrent/session.cpp
    src/bittorrent/torrentinfo.cpp
    src/bittorrent/torrenthandle.cpp
    src/bittorrent/torrentstatus.cpp
    src/bittorrent/torrentsubsystem.cpp
    src/http/defaultrequesthandlerfactory.cpp
    src/http/jsonrpcrequesthandler.cpp
    src/http/httpsubsystem.cpp
    src/http/websocketrequesthandler.cpp
    src/http/jsonrpc/sessionaddtorrentfilemethod.cpp
    src/http/jsonrpc/sessiongettorrentsmethod.cpp
    src/http/jsonrpc/torrentgetfilesmethod.cpp
    src/http/jsonrpc/torrentpausemethod.cpp
    src/http/jsonrpc/torrentresumemethod.cpp
)

target_link_libraries(
    hadouken
    ${LIBTORRENT}
    ${LIBBOOST}
    ${LIBPOCO}
    ${PTHREAD}
)
