cmake_minimum_required(VERSION 2.8 FATAL_ERROR)

project("hadouken")

# Put the libaries and binaries that get built into directories at the
# top of the build tree rather than in hard-to-find leaf
# directories. This simplifies manual testing and the use of the build
# tree rather than installed Boost libraries.
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib)
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib)
# Windows DLLs are "runtime" for CMake. Output them to "bin" like the Visual Studio projects do.
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)

if(WIN32)
    # Set dependency directories
    set(BOOST_ROOT      $ENV{BOOST_ROOT})
    set(LIBTORRENT_ROOT $ENV{LIBTORRENT_ROOT})
    set(OPENSSL_ROOT    $ENV{OPENSSL_ROOT})

    set(LIBSSL libeay32.lib ssleay32.lib)
    set(LIBEXTRAS iphlpapi.lib dbghelp.lib shlwapi.lib)
    set(LIBTORRENT libtorrent.lib)

    add_definitions(
        -DUNICODE
        -DWIN32
        -DWIN32_LEAN_AND_MEAN
        -D_CRT_SECURE_NO_DEPRECATE
        -D_FILE_OFFSET_BITS=64
        -D_SCL_SECURE_NO_DEPRECATE
        -D_UNICODE
        -D_WIN32
        -D_WIN32_WINNT=0x0600
        -D__USE_W32_SOCKETS
        /Zc:wchar_t
        /Zc:forScope
        /MP
    )

    include_directories(
        ${BOOST_ROOT}
        ${LIBTORRENT_ROOT}/include
        ${OPENSSL_ROOT}/inc32
    )

    link_directories(
        ${BOOST_ROOT}/stage/lib
        ${LIBTORRENT_ROOT}/bin/msvc-14.0/$(ConfigurationName)/address-model-64/crypto-openssl/deprecated-functions-off/link-static/threading-multi
        ${OPENSSL_ROOT}/out32
    )

    # Make sure release builds have pdb files.
    set(CMAKE_SHARED_LINKER_FLAGS_RELEASE "${CMAKE_SHARED_LINKER_FLAGS_RELEASE} /DEBUG /OPT:REF")
    set(CMAKE_EXE_LINKER_FLAGS_RELEASE    "${CMAKE_EXE_LINKER_FLAGS_RELEASE}    /DEBUG /OPT:REF")
else()
    set(LIBBOOST libboost_system.a libboost_log.a libboost_program_options.a libboost_filesystem.a libboost_thread.a)
    set(LIBEXTRAS rt)
    set(LIBSSL ssl crypto)
    set(LIBTORRENT libtorrent-rasterbar.a)
    set(PTHREAD pthread)

    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")
    
    if(DEFINED ENV{TRAVIS})
        # Disable deprecated warnings (for auto_ptr mainly) on travis
        # since there are so many and travis cuts our log output after
        # 10k lines.
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wno-deprecated")
    endif()
endif()

include_directories(
    ${CMAKE_SOURCE_DIR}/deps/cpp-netlib
)

# add_definitions() doesn't seem to let you say wich build type to apply it to
set(CMAKE_C_FLAGS_DEBUG "${CMAKE_C_FLAGS_DEBUG} -DTORRENT_DEBUG")

# Get our version number
file (STRINGS "${CMAKE_SOURCE_DIR}/VERSION" VERSION)

# Get the current working branch
execute_process(
  COMMAND git rev-parse --abbrev-ref HEAD
  WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
  OUTPUT_VARIABLE GIT_BRANCH
  OUTPUT_STRIP_TRAILING_WHITESPACE
)

# Get the latest abbreviated commit hash of the working branch
execute_process(
  COMMAND git log -1 --format=%h
  WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
  OUTPUT_VARIABLE GIT_COMMIT_HASH
  OUTPUT_STRIP_TRAILING_WHITESPACE
)

# Generate version.hpp
configure_file("${CMAKE_SOURCE_DIR}/src/version.cpp.in" "${CMAKE_SOURCE_DIR}/src/version.cpp" @ONLY)

add_definitions(
    -DBOOST_ASIO_ENABLE_CANCELIO
    -DBOOST_ASIO_HASH_MAP_BUCKETS=1021
    -DBOOST_EXCEPTION_DISABLE
    -DBOOST_NETWORK_ENABLE_HTTPS
    -DTORRENT_USE_I2P=1
    -DTORRENT_USE_OPENSSL
    -DTORRENT_NO_DEPRECATE
    -DWITH_SHIPPED_GEOIP_H
)

include_directories(
    ${CMAKE_SOURCE_DIR}/include
)

set(HADOUKEN_SOURCES
    src/application
    src/main
    src/version
    src/hosting/console_host
    src/http/connection_handler
    src/http/http_server
    src/http/api_request_handler
    src/http/gui_request_handler
    src/scripting/duktape
    src/scripting/script_host
    src/scripting/modules/bencoding_module
    src/scripting/modules/bittorrent_module
    src/scripting/modules/core_module
    src/scripting/modules/file_system_module
    src/scripting/modules/http_module
    src/scripting/modules/logger_module
    src/scripting/modules/process_module
    src/scripting/modules/bittorrent/add_torrent_params_wrapper
    src/scripting/modules/bittorrent/alert_wrapper
    src/scripting/modules/bittorrent/announce_entry_wrapper
    src/scripting/modules/bittorrent/entry_wrapper
    src/scripting/modules/bittorrent/error_code_wrapper
    src/scripting/modules/bittorrent/feed_handle_wrapper
    src/scripting/modules/bittorrent/feed_settings_wrapper
    src/scripting/modules/bittorrent/feed_status_wrapper
    src/scripting/modules/bittorrent/lazy_entry_wrapper
    src/scripting/modules/bittorrent/peer_info_wrapper
    src/scripting/modules/bittorrent/session_handle_wrapper
    src/scripting/modules/bittorrent/settings_pack_wrapper
    src/scripting/modules/bittorrent/torrent_creator_wrapper
    src/scripting/modules/bittorrent/torrent_handle_wrapper
    src/scripting/modules/bittorrent/torrent_info_wrapper
    src/scripting/modules/bittorrent/torrent_status_wrapper
)

# Append platform-specific sources
if(WIN32)
    list(APPEND HADOUKEN_SOURCES src/platform_win32 src/hosting/service_host)
else()
    list(APPEND HADOUKEN_SOURCES src/platform_unix)
endif(WIN32)

set(CPP_NETLIB_SOURCES
    ${CMAKE_SOURCE_DIR}/deps/cpp-netlib/libs/network/src/client
    ${CMAKE_SOURCE_DIR}/deps/cpp-netlib/libs/network/src/server_request_parsers_impl
    ${CMAKE_SOURCE_DIR}/deps/cpp-netlib/libs/network/src/uri/schemes
    ${CMAKE_SOURCE_DIR}/deps/cpp-netlib/libs/network/src/uri/uri
)

add_executable(
    hadouken
    ${HADOUKEN_SOURCES}
    ${CPP_NETLIB_SOURCES}
)

target_link_libraries(
    hadouken
    ${LIBTORRENT}
    ${LIBEXTRAS}
    ${LIBBOOST}
    ${LIBSSL}
    ${PTHREAD}
)

if(WIN32)
    target_link_libraries(
        hadouken
        debug libboost_random-vc140-mt-gd-1_59.lib
    )

    target_link_libraries(
        hadouken
        optimized libboost_random-vc140-mt-1_59.lib
    )
endif(WIN32)

if(UNIX)
    install(TARGETS hadouken DESTINATION bin)
    install(FILES ${CMAKE_SOURCE_DIR}/linux/build/bin/webui.zip DESTINATION share/hadouken)
    install(FILES ${CMAKE_SOURCE_DIR}/dist/linux/hadouken.json.template DESTINATION /etc/hadouken)
    install(DIRECTORY ${CMAKE_SOURCE_DIR}/js DESTINATION share/hadouken)

    # To generate deb packages.
    set(CPACK_GENERATOR "DEB")
    set(CPACK_PACKAGE_VERSION "${VERSION}")
    set(CPACK_STRIP_FILES "bin/hadouken")
    set(CPACK_DEBIAN_PACKAGE_MAINTAINER "Viktor Elofsson and contributors.")
    include(CPack)
endif(UNIX)
