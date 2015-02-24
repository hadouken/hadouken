cmake_minimum_required(VERSION 2.8 FATAL_ERROR)

project("autoadd")

add_library(
    autoadd
    SHARED
    extensions/autoadd/manifest.cpp
    extensions/autoadd/autoaddextension.cpp
)

target_link_libraries(
    autoadd
    hadouken
)
