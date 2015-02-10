cmake_minimum_required(VERSION 2.8 FATAL_ERROR)

project("hadouken")

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_SOURCE_DIR}/bin)

add_compile_options(-std=c++0x) 

add_definitions(
    -DBOOST_ASIO_DYN_LINK
    -DBOOST_ALL_DYN_LINK
    -DTORRENT_NO_DEPRECATE
)

if(WIN32)
    add_definitions(
        -DBOOST_ALL_NO_LIB
        -DWIN32
        -DWIN32_LEAN_AND_MEAN
        -D_WIN32_WINNT=0x0501
    )
endif()

include_directories(
    include
)

if(WIN32)
    include_directories(
        ${CMAKE_SOURCE_DIR}/packages/hadouken.boost.0.1.5/include
        ${CMAKE_SOURCE_DIR}/packages/hadouken.openssl.0.1.3/win32/debug/include
        ${CMAKE_SOURCE_DIR}/packages/hadouken.libtorrent.0.1.3/win32/debug/include
    )
endif()

if(WIN32)
    link_directories(
        ${CMAKE_SOURCE_DIR}/packages/hadouken.boost.0.1.5/win32/debug/lib
        ${CMAKE_SOURCE_DIR}/packages/hadouken.libtorrent.0.1.3/win32/debug/lib
        ${CMAKE_SOURCE_DIR}/packages/hadouken.openssl.0.1.3/win32/debug/lib
    )
endif()

add_library(
    hadouken
    SHARED
    src/bt_session.cpp
    src/bt_torrent_handle.cpp
    src/plugin_manager.cpp
    src/service_locator.cpp
)

if(WIN32)
    target_link_libraries(
        hadouken
        torrent.lib
        boost_date_time.lib
        boost_filesystem.lib
        boost_log.lib
        boost_program_options.lib
        boost_regex.lib
        boost_system.lib
    )
else()
    target_link_libraries(
        hadouken
        libtorrent-rasterbar.so
        libboost_system.so
        libboost_filesystem.so
        libboost_log.so
    )
endif()

add_executable(
    hadoukend
    src/host.hpp
    src/console_host.hpp
    src/console_host.cpp
    src/service_host.hpp
    src/service_host.cpp
    src/main.cpp
)

target_link_libraries(
    hadoukend
    hadouken
)

add_subdirectory(plugins/auto_move)
add_subdirectory(plugins/folder_watcher)
