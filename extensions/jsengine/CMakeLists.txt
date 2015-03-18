cmake_minimum_required(VERSION 2.8 FATAL_ERROR)

project("jsengine")

add_library(
    jsengine
    SHARED
    duktape.c
    manifest.cpp
    jsengineextension.cpp
    mod_fs.cpp
)

target_link_libraries(
    jsengine
    hadouken
)
