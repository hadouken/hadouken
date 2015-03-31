cmake_minimum_required(VERSION 2.8 FATAL_ERROR)

project("pushbullet")

add_library(
    pushbullet
    SHARED
    manifest.cpp
    pushbulletextension.cpp
)

target_link_libraries(
    pushbullet
    hadouken
)

install(TARGETS pushbullet DESTINATION lib/hadoukend)
