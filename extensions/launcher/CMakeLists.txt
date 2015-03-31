cmake_minimum_required(VERSION 2.8 FATAL_ERROR)

project("launcher")

add_library(
    launcher
    SHARED
    manifest.cpp
    launcherextension.cpp
)

target_link_libraries(
    launcher
    hadouken
)

install(TARGETS launcher DESTINATION lib/hadoukend)
