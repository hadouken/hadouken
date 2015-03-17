cmake_minimum_required(VERSION 2.8 FATAL_ERROR)

project("jsengine")

add_library(
    jsengine
    SHARED
    manifest.cpp
    jsengineextension.cpp
    duktape.c
)

target_link_libraries(
    jsengine
    hadouken
)
