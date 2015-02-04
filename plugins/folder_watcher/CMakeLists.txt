cmake_minimum_required(VERSION 3.1.1 FATAL_ERROR)

link_directories(
    ${CMAKE_BINARY_DIR}
)

add_library(
    folder_watcher
    SHARED
    folder_watcher.cpp
    folder_Watcher.hpp
)

target_link_libraries(
    folder_watcher
    hadouken
)
