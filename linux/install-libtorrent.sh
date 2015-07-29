#!/bin/sh
set -e
# check to see if libtorrent folder is empty
if [ ! -d "$HOME/libtorrent/lib" ]; then
  wget https://github.com/arvidn/libtorrent/archive/libtorrent-1_0_5.tar.gz
  tar -xzvf libtorrent-1_0_5.tar.gz > /dev/null
  cd libtorrent-libtorrent-1_0_5

  ./autotool.sh
  ./configure --with-boost=$HOME/boost --enable-debug=no --enable-deprecated-functions=no --prefix=$HOME/libtorrent
  make
  make install
else
  echo "Using cached libtorrent directory."
fi
