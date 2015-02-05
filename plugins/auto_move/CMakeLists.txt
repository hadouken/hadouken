cmake_minimum_required(VERSION 2.8 FATAL_ERROR)

link_directories(
    ${CMAKE_BINARY_DIR}
)

add_library(
    auto_move
    SHARED
    auto_move.cpp
    auto_move.hpp
)

target_link_libraries(
    auto_move
    hadouken
)
