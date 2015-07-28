#!/bin/sh
set -e
# check to see if boost folder is empty
if [ ! -d "$HOME/boost/lib" ]; then
  wget http://downloads.sourceforge.net/project/boost/boost/1.58.0/boost_1_58_0.tar.gz
  tar -xzvf boost_1_58_0.tar.gz > /dev/null
  echo "using gcc : 4.8 : /usr/bin/g++-4.8 ; " >> $HOME/user-config.jam
  cd boost_1_58_0 && ./bootstrap.sh toolset=gcc-4.8 --prefix=$HOME/boost --with-libraries=system,log,filesystem,program_options,thread && ./b2 toolset=gcc-4.8 install >> /dev/null
else
  echo "Using cached boost directory."
fi
