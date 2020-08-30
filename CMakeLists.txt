cmake_minimum_required(VERSION 3.16 FATAL_ERROR)

project("hadouken")

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# --------- boost options
set(Boost_USE_MULTITHREADED TRUE)
set(Boost_USE_STATIC_LIBS ON)
# ---------

# --------- libtorrent options
option(BUILD_SHARED_LIBS    "" off)
option(deprecated-functions "" off)
# ----------------------------

# --------- nlohmann-json options
set(JSON_BuildTests OFF CACHE INTERNAL "")
# ---------

find_package(Boost COMPONENTS log program_options system)

add_subdirectory(vendor/libtorrent)
add_subdirectory(vendor/nlohmann-json)

add_executable(
    hadouken
    src/hadouken/main
    src/hadouken/session_manager

    # jsonrpc
    src/hadouken/jsonrpc/jsonrpc_server
    src/hadouken/jsonrpc/methods/session_get_settings
    src/hadouken/jsonrpc/methods/torrents_add

    # http server
    src/hadouken/http/http_server
    src/hadouken/http/http_session
)

set_target_properties(hadouken PROPERTIES CXX_VISIBILITY_PRESET hidden)

target_link_libraries(
    hadouken
    PRIVATE
    ${Boost_LIBRARIES}
    nlohmann_json::nlohmann_json
    "torrent-rasterbar"
)
