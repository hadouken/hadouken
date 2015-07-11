#!/bin/bash

# Script variables
DEPS_DIR="./deps" # where should we put our dependencies.

LT_REPO="https://github.com/arvidn/libtorrent"
LT_DIR="libtorrent"
LT_TAG="libtorrent-1_0_5"

CPPNL_URL="http://storage.googleapis.com/cpp-netlib-downloads/0.11.1/cpp-netlib-0.11.1-final.zip"
CPPNL_DIR_ORIG="cpp-netlib-0.11.1-final"
CPPNL_DIR_NEW="cpp-netlib"
CPPNL_FILE="cpp-netlib.zip"

# Create DEPS_DIR if it doesn't exist
mkdir -p $DEPS_DIR

# Clone libtorrent to DEPS_DIR
if [[ ! -d "$DEPS_DIR/$LT_DIR" ]]; then
    ( cd $DEPS_DIR ; git clone $LT_REPO )
fi

# Checkout the correct libtorrent tag
( cd $DEPS_DIR ; cd $LT_DIR ; git checkout $LT_TAG )

# Download cpp-netlib
if [[ ! -f "$DEPS_DIR/$CPPNL_FILE" ]]; then
    wget $CPPNL_URL -O "$DEPS_DIR/$CPPNL_FILE"
fi

# unzip cpp netlib
if [[ ! -d "$DEPS_DIR/$CPPNL_DIR_NEW" ]]; then
    ( cd $DEPS_DIR ; unzip $CPPNL_FILE >> /dev/null ; mv $CPPNL_DIR_ORIG $CPPNL_DIR_NEW )
fi
