#!/bin/bash

cd "$(dirname "$0")"

export CC=/usr/bin/gcc-4.8
export CXX=/usr/bin/g++-4.8
export CPATH=$HOME/boost/include
export CPATH=$CPATH:$HOME/libtorrent/include
export LIBRARY_PATH=$HOME/boost/lib
export LIBRARY_PATH=$LIBRARY_PATH:$HOME/libtorrent/lib

( mkdir -p build ; cd build ; cmake ../../ ; make )
