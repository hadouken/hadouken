cmake_minimum_required(VERSION 2.8 FATAL_ERROR)

project("hadouken")

# Put the libaries and binaries that get built into directories at the
# top of the build tree rather than in hard-to-find leaf
# directories. This simplifies manual testing and the use of the build
# tree rather than installed Boost libraries.
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib)
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib)
# Windows DLLs are "runtime" for CMake. Output them to "bin" like the Visual Studio projects do.
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)

if(WIN32)
    set(LIBTORRENT torrent.lib)

    add_definitions(
        -DPOCO_NO_UNWINDOWS
        -DWIN32
        -DWIN32_LEAN_AND_MEAN
        -D_CRT_SECURE_NO_WARNINGS
        -D_SCL_SECURE_NO_WARNINGS
        -D_WIN32_WINNT=0x0501
    )

    include_directories(
        ${CMAKE_SOURCE_DIR}/win32/libs/hadouken.libtorrent/win32/include
        ${CMAKE_SOURCE_DIR}/win32/libs/hadouken.openssl/win32/include
        ${CMAKE_SOURCE_DIR}/win32/libs/hadouken.poco/win32/include
    )

    link_directories(
        ${CMAKE_SOURCE_DIR}/win32/libs/hadouken.libtorrent/win32/lib
        ${CMAKE_SOURCE_DIR}/win32/libs/hadouken.openssl/win32/lib
        ${CMAKE_SOURCE_DIR}/win32/libs/hadouken.poco/win32/lib
    )

    # Make sure release builds have pdb files.
    set(CMAKE_SHARED_LINKER_FLAGS_RELEASE "${CMAKE_SHARED_LINKER_FLAGS_RELEASE} /DEBUG /OPT:REF")
    set(CMAKE_EXE_LINKER_FLAGS_RELEASE    "${CMAKE_EXE_LINKER_FLAGS_RELEASE}    /DEBUG /OPT:REF")
else()
    set(LIBBOOST boost_system)
    set(LIBTORRENT libtorrent-rasterbar.so)
    set(LIBPOCO libPocoFoundation.so libPocoJSON.so libPocoNet.so libPocoNetSSL.so libPocoUtil.so)
    set(PTHREAD pthread)

    add_compile_options(-std=c++0x)
endif()

# Get our version number
file (STRINGS "${CMAKE_SOURCE_DIR}/VERSION" VERSION)

# Get the current working branch
execute_process(
  COMMAND git rev-parse --abbrev-ref HEAD
  WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
  OUTPUT_VARIABLE GIT_BRANCH
  OUTPUT_STRIP_TRAILING_WHITESPACE
)

# Get the latest abbreviated commit hash of the working branch
execute_process(
  COMMAND git log -1 --format=%h
  WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
  OUTPUT_VARIABLE GIT_COMMIT_HASH
  OUTPUT_STRIP_TRAILING_WHITESPACE
)

# Generate version.hpp
configure_file("${CMAKE_SOURCE_DIR}/src/version.cpp.in" "${CMAKE_SOURCE_DIR}/src/version.cpp" @ONLY)

add_definitions(
    -DBOOST_ASIO_SEPARATE_COMPILATION
    -DBOOST_ALL_DYN_LINK
    -DTORRENT_NO_DEPRECATE
    -DTORRENT_USE_OPENSSL
)

include_directories(
    ${CMAKE_SOURCE_DIR}/include
)

# Get sources for hadouken (except main.cpp)
file(GLOB_RECURSE HADOUKEN_SOURCES ${CMAKE_SOURCE_DIR}/src/*.cpp)
list(REMOVE_ITEM  HADOUKEN_SOURCES ${CMAKE_SOURCE_DIR}/src/main.cpp)

add_library(
    hadouken
    SHARED
    ${HADOUKEN_SOURCES}
)

add_executable(
    hadoukend
    src/main.cpp
)

target_link_libraries(
    hadouken
    ${LIBTORRENT}
    ${LIBBOOST}
    ${LIBPOCO}
    ${PTHREAD}
)

target_link_libraries(
    hadoukend
    hadouken
    ${LIBTORRENT}
    ${LIBBOOST}
    ${LIBPOCO}
    ${PTHREAD}
)

# Extensions
add_subdirectory(extensions/autoadd)
add_subdirectory(extensions/automove)
add_subdirectory(extensions/jsengine)
add_subdirectory(extensions/pushbullet)
