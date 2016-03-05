#!/bin/bash

cd "$(dirname "$0")"

export CC=/usr/bin/gcc-4.8
export CXX=/usr/bin/g++-4.8
export CPATH=$HOME/boost/include
export CPATH=$CPATH:$HOME/libtorrent/include
export LIBRARY_PATH=$HOME/boost/lib
export LIBRARY_PATH=$LIBRARY_PATH:$HOME/libtorrent/lib

# Prepare output path
( mkdir -p build/bin )

# Create webui.zip package
( cd ../webui ; zip -r ../linux/build/bin/webui.zip . )

# Copy JS folder
( cp -R ../js build/bin/ )

# Build and package Hadouken
( mkdir -p build ; cd build ; cmake ../../ -DBOOST_ROOT=$HOME/boost ; make ; make package )
