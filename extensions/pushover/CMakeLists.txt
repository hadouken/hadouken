cmake_minimum_required(VERSION 2.8 FATAL_ERROR)

project("pushover")

add_library(
    pushover
    SHARED
    manifest.cpp
    pushoverextension.cpp
)

target_link_libraries(
    pushover
    hadouken
)

install(TARGETS pushover DESTINATION lib/hadoukend)
