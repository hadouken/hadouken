cmake_minimum_required(VERSION 2.8 FATAL_ERROR)

project("automove")

add_library(
    automove
    SHARED
    manifest.cpp
    automoveextension.cpp
)

target_link_libraries(
    automove
    hadouken
)
