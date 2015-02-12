cmake_minimum_required(VERSION 2.8 FATAL_ERROR)

project("hadouken")

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_SOURCE_DIR}/bin)

if(NOT WIN32)
    add_compile_options(-std=c++0x) 
endif()

if(NOT BUILD_CONFIGURATION)
    set(BUILD_CONFIGURATION "debug")
endif()

message("Build configuration: ${BUILD_CONFIGURATION}")

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
        ${CMAKE_SOURCE_DIR}/packages/hadouken.boost.0.1.8/include
        ${CMAKE_SOURCE_DIR}/packages/hadouken.openssl.0.1.3/win32/${BUILD_CONFIGURATION}/include
        ${CMAKE_SOURCE_DIR}/packages/hadouken.libtorrent.0.1.3/win32/${BUILD_CONFIGURATION}/include
    )
endif()

if(WIN32)
    link_directories(
        ${CMAKE_SOURCE_DIR}/packages/hadouken.boost.0.1.8/win32/${BUILD_CONFIGURATION}/lib
        ${CMAKE_SOURCE_DIR}/packages/hadouken.libtorrent.0.1.3/win32/${BUILD_CONFIGURATION}/lib
        ${CMAKE_SOURCE_DIR}/packages/hadouken.openssl.0.1.3/win32/${BUILD_CONFIGURATION}/lib
    )
endif()

add_library(
    hadouken
    SHARED
    src/bittorrent/session.cpp
    src/bittorrent/torrent_handle.cpp
    src/http/connection.cpp
    src/http/connection_manager.cpp
    src/http/http_server.cpp
    src/http/reply.cpp
    src/http/request_handler.cpp
    src/http/request_parser.cpp
    src/logger.cpp
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
        boost_thread.lib
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
    src/hosting/console_host.cpp
    src/hosting/service_host.cpp
    src/main.cpp
)

target_link_libraries(
    hadoukend
    hadouken
)
