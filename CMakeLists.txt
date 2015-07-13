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
    set(LIBSSL libeay32.lib ssleay32.lib)
    set(LIBEXTRAS iphlpapi.lib dbghelp.lib shlwapi.lib)

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
        ${CMAKE_SOURCE_DIR}/win32/libs/hadouken.boost/include
        ${CMAKE_SOURCE_DIR}/win32/libs/hadouken.openssl/win32/include
    )

    link_directories(
        ${CMAKE_SOURCE_DIR}/win32/libs/hadouken.boost/win32/lib
        ${CMAKE_SOURCE_DIR}/win32/libs/hadouken.openssl/win32/lib
    )

    # Make sure release builds have pdb files.
    set(CMAKE_SHARED_LINKER_FLAGS_RELEASE "${CMAKE_SHARED_LINKER_FLAGS_RELEASE} /DEBUG /OPT:REF")
    set(CMAKE_EXE_LINKER_FLAGS_RELEASE    "${CMAKE_EXE_LINKER_FLAGS_RELEASE}    /DEBUG /OPT:REF")
else()
    set(LIBBOOST libboost_system.a libboost_log.a libboost_program_options.a libboost_filesystem.a libboost_thread.a)
    set(LIBEXTRAS rt)
    set(LIBSSL ssl crypto)
    set(PTHREAD pthread)

    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++0x")
    
    if(DEFINED ENV{TRAVIS})
        # Disable deprecated warnings (for auto_ptr mainly) on travis
        # since there are so many and travis cuts our log output after
        # 10k lines.
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wno-deprecated")
    endif()
endif()

include_directories(
    ${CMAKE_SOURCE_DIR}/deps/cpp-netlib
    ${CMAKE_SOURCE_DIR}/deps/libtorrent/include
    ${CMAKE_SOURCE_DIR}/deps/libtorrent/ed25519/src
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
    -DBOOST_ASIO_SEPARATE_COMPILATION
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

set(ltdir ${CMAKE_SOURCE_DIR}/deps/libtorrent)

# Get sources for libtorrent
set(sources
    web_connection_base
    alert
    alert_manager
    allocator
    asio
    assert
    bandwidth_limit
    bandwidth_manager
    bandwidth_queue_entry
    bloom_filter
    chained_buffer
    connection_queue
    create_torrent
    disk_buffer_holder
    entry
    error_code
    file_storage
    lazy_bdecode
    escape_string
    string_util
    file
    gzip
    hasher
    http_connection
    http_stream
    http_parser
    i2p_stream
    identify_client
    ip_filter
    ip_voter
    peer_connection
    bt_peer_connection
    web_peer_connection
    http_seed_connection
    instantiate_connection
    natpmp
    packet_buffer
    piece_picker
    policy
    puff
    random
    rss
    session
    session_impl
    settings
    socket_io
    socket_type  
    socks5_stream
    stat
    storage
    time
    timestamp_history
    torrent
    torrent_handle
    torrent_info
    tracker_manager
    http_tracker_connection
    utf8
    udp_tracker_connection
    udp_socket
    upnp
    utp_socket_manager
    utp_stream
    logger
    file_pool
    lsd
    disk_buffer_pool
    disk_io_thread
    enum_net
    broadcast_socket
    magnet_uri
    parse_url
    ConvertUTF
    thread
    xml_parse
    pe_crypto
    asio_ssl
    GeoIP

# -- extensions --
    metadata_transfer
    ut_pex
    ut_metadata
    smart_ban
    lt_trackers
)

# -- kademlia --
set(kademlia_sources
    dht_tracker
    node
    refresh
    rpc_manager
    find_data
    node_id
    routing_table
    traversal_algorithm
    logging
    item
    get_peers
    get_item
)

# -- ed25519 --
set(ed25519_sources
    add_scalar
    fe
    ge
    key_exchange
    keypair
    sc
    seed
    sha512
    sign
    verify
)

foreach(s ${sources})
    list(APPEND LIBTORRENT_SOURCES ${ltdir}/src/${s})
endforeach(s)

foreach(s ${kademlia_sources})
    list(APPEND LIBTORRENT_SOURCES ${ltdir}/src/kademlia/${s})
endforeach(s)
foreach(s ${ed25519_sources})
    list(APPEND LIBTORRENT_SOURCES ${ltdir}/ed25519/src/${s})
endforeach(s)


set(HADOUKEN_SOURCES
    src/application
    src/main
    src/version
    src/hosting/console_host
    src/http/connection_handler
    src/http/http_server
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
    src/scripting/modules/bittorrent/session_settings_wrapper
    src/scripting/modules/bittorrent/session_wrapper
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
    ${LIBTORRENT_SOURCES}
    ${CPP_NETLIB_SOURCES}
)

target_link_libraries(
    hadouken
    ${LIBEXTRAS}
    ${LIBBOOST}
    ${LIBSSL}
    ${PTHREAD}
)
