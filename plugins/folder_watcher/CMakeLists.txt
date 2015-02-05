cmake_minimum_required(VERSION 2.8 FATAL_ERROR)

link_directories(
    ${CMAKE_BINARY_DIR}
)

add_library(
    folder_watcher
    SHARED
    folder_watcher.cpp
    folder_watcher.hpp
)

target_link_libraries(
    folder_watcher
    hadouken
)
