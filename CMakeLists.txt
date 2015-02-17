cmake_minimum_required(VERSION 2.8 FATAL_ERROR)

project("hadouken")

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_SOURCE_DIR}/bin)

if(NOT BUILD_CONFIGURATION)
    set(BUILD_CONFIGURATION "debug")
endif()

message("Build configuration: ${BUILD_CONFIGURATION}")

if(WIN32)
    set(BOOST_DATE_TIME "boost_date_time.lib")
    set(BOOST_FILESYSTEM "boost_filesystem.lib")
    set(BOOST_LOG "boost_log.lib")
    set(BOOST_PROGRAM_OPTIONS "boost_program_options.lib")
    set(BOOST_REGEX "boost_regex.lib")
    set(BOOST_SYSTEM "boost_system.lib")
    set(BOOST_THREAD "boost_thread.lib")
    set(LIBTORRENT "torrent.lib")
    set(PTHREAD "")
    set(V8 "v8.lib")

    add_definitions(
        -DBOOST_ALL_NO_LIB
        -DWIN32
        -DWIN32_LEAN_AND_MEAN
        -D_CRT_SECURE_NO_WARNINGS
        -D_SCL_SECURE_NO_WARNINGS
        -D_WIN32_WINNT=0x0501
    )

    include_directories(
        ${CMAKE_SOURCE_DIR}/packages/hadouken.boost.0.1.8/include
        ${CMAKE_SOURCE_DIR}/packages/hadouken.openssl.0.1.3/win32/${BUILD_CONFIGURATION}/include
        ${CMAKE_SOURCE_DIR}/packages/hadouken.libtorrent.0.1.3/win32/${BUILD_CONFIGURATION}/include
        ${CMAKE_SOURCE_DIR}/packages/hadouken.v8.0.1.4/win32/${BUILD_CONFIGURATION}/include
    )

    link_directories(
        ${CMAKE_SOURCE_DIR}/packages/hadouken.boost.0.1.8/win32/${BUILD_CONFIGURATION}/lib
        ${CMAKE_SOURCE_DIR}/packages/hadouken.libtorrent.0.1.3/win32/${BUILD_CONFIGURATION}/lib
        ${CMAKE_SOURCE_DIR}/packages/hadouken.openssl.0.1.3/win32/${BUILD_CONFIGURATION}/lib
        ${CMAKE_SOURCE_DIR}/packages/hadouken.v8.0.1.4/win32/${BUILD_CONFIGURATION}/lib
    )
else()
    set(BOOST_DATE_TIME "libboost_date_time.so")
    set(BOOST_FILESYSTEM "libboost_filesystem.so")
    set(BOOST_LOG "libboost_log.so")
    set(BOOST_PROGRAM_OPTIONS "libboost_program_options.so")
    set(BOOST_REGEX "libboost_regex.so")
    set(BOOST_SYSTEM "libboost_system.so")
    set(BOOST_THREAD "libboost_thread.so")
    set(LIBTORRENT "libtorrent-rasterbar.so")
    set(PTHREAD "pthread")
    set(V8 "libv8.so")

    add_compile_options(-std=c++0x) 
endif()

add_definitions(
    -DBOOST_ASIO_SEPARATE_COMPILATION
    -DBOOST_ALL_DYN_LINK
    -DTORRENT_NO_DEPRECATE
)

include_directories(
    include
)

add_library(
    hadouken_common
    SHARED
    src/common.cpp
    src/logger.cpp
    src/service_locator.cpp
)

add_library(
    hadouken_bittorrent
    SHARED
    src/bittorrent/session.cpp
    src/bittorrent/torrent_handle.cpp
)

add_library(
    hadouken_http
    SHARED
    src/http/connection.cpp
    src/http/connection_manager.cpp
    src/http/http.cpp
    src/http/http_parser.c
    src/http/http_server.cpp
    src/http/reply.cpp
)

add_library(
    hadouken_scripting
    SHARED
    src/scripting/js_engine.cpp
)

target_link_libraries(
    hadouken_common
    ${BOOST_DATE_TIME}
    ${BOOST_FILESYSTEM}
    ${BOOST_LOG}
    ${BOOST_SYSTEM}
    ${BOOST_THREAD}
)

target_link_libraries(
    hadouken_bittorrent
    hadouken_common
    ${LIBTORRENT}
    ${BOOST_DATE_TIME}
    ${BOOST_FILESYSTEM}
    ${BOOST_LOG}
    ${BOOST_REGEX}
    ${BOOST_SYSTEM}
)

target_link_libraries(
    hadouken_http
    hadouken_common
    ${BOOST_DATE_TIME}
    ${BOOST_SYSTEM}
    ${BOOST_THREAD}
)

target_link_libraries(
    hadouken_scripting
    hadouken_common
    ${V8}
)

add_executable(
    hadoukend
    src/hosting/console_host.cpp
    src/hosting/service_host.cpp
    src/main.cpp
)

target_link_libraries(
    hadoukend
    hadouken_common
    hadouken_bittorrent
    hadouken_http
    hadouken_scripting
    ${BOOST_PROGRAM_OPTIONS}
    ${PTHREAD}
)
