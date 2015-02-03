cmake_minimum_required(VERSION 3.1.1 FATAL_ERROR)

project("hadouken")

add_definitions(
    -DBOOST_ASIO_DYN_LINK
    -DBOOST_ALL_DYN_LINK
    -DTORRENT_NO_DEPRECATE
    -DWIN32
)

include_directories(
    include
    $ENV{BOOST_ROOT}
    $ENV{LIBTORRENT_ROOT}/include
    $ENV{OPENSSL_ROOT}/include
)

link_directories(
    $ENV{BOOST_ROOT}/stage/lib
    $ENV{LIBTORRENT_ROOT}/build/debug.x86
    $ENV{OPENSSL_ROOT}/build/debug.x86/lib
)

add_executable(
    hadouken
    src/bt_session.cpp
    src/bt_torrent_handle.cpp
    src/main.cpp
    src/plugin_manager.cpp
    src/plugin_manager.hpp
    src/service_locator.cpp
)

target_link_libraries(
    hadouken
    torrent.lib
)
