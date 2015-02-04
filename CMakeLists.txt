cmake_minimum_required(VERSION 3.1.1 FATAL_ERROR)

project("hadouken")

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_SOURCE_DIR}/bin)

add_definitions(
    -DBOOST_ASIO_DYN_LINK
    -DBOOST_ALL_DYN_LINK
    -DTORRENT_NO_DEPRECATE
    -DWIN32
    -DWIN32_LEAN_AND_MEAN
    -D_WIN32_WINNT=0x0501
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

add_library(
    hadouken
    SHARED
    src/bt_session.cpp
    src/bt_torrent_handle.cpp
    src/plugin_manager.cpp
    src/service_locator.cpp
)

target_link_libraries(
    hadouken
    torrent.lib
)

add_executable(
    hadoukend
    src/main.cpp
)

target_link_libraries(
    hadoukend
    hadouken
)

add_subdirectory(plugins/auto_move)
add_subdirectory(plugins/folder_watcher)
