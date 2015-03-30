
Installing Hadouken on Ubuntu
=============================

Overview
--------

Running Hadouken on Ubuntu (or on Debian/Debian derivatives) requires you
to compile your own binaries.

.. note:: In the future we will try to provide a PPA with prebuilt packages.


Preparing your environment
``````````````````````````

To successfully build and run Hadouken you need a few base packages installed.
This guide assumes a clean Ubuntu 14.04 LTS environment with the following
extra packages,

* :code:`build-essential` - provides g++ and other required build tools.
* :code:`cmake` - the build system Hadouken uses.
* :code:`git` - to get the latest Hadouken sources.
* :code:`libboost-system-dev` - required for Rasterbar-libtorrent.
* :code:`libssl-dev` - for HTTPS and SSL functionality in both Hadouken and
  Rasterbar-libtorrent.


Building dependencies
---------------------

There are a few dependencies for Hadouken which you need to build manually.


Rasterbar-libtorrent
````````````````````

.. code:: bash

   $ wget http://sourceforge.net/projects/libtorrent/files/libtorrent/libtorrent-rasterbar-1.0.4.tar.gz
   $ tar -xvzf libtorrent-rasterbar-1.0.4.tar.gz
   $ cd libtorrent-rasterbar-1.0.4/
   $ ./configure --disable-deprecated-functions
   $ make
   $ sudo make install


Poco
````

.. code:: bash

  $ wget https://github.com/pocoproject/poco/archive/poco-1.6.0-release.tar.gz
  $ tar -xvzf poco-1.6.0-release.tar.gz
  $ cd poco-poco-1.6.0-release/
  $ ./configure --no-samples --no-tests --omit=Data/MySQL,Data/ODBC
  $ make
  $ sudo make install


Building Hadouken
-----------------

With all dependencies compiled and installed, Hadouken is easy to build. Clone
the latest code and build with *cmake*.

.. code:: bash

  $ git clone https://github.com/hadouken/hadouken
  $ cd hadouken/
  $ mkdir cmake-build
  $ cd cmake-build/
  $ cmake ..
  $ make

After running these commands, you will have a binary, :file:`hadoukend` in the
*bin* folder. All extensions are put in the *lib* folder.


Enabling extensions
```````````````````

Hadouken will look in the application directory for extensions, which means
that :file:`libautoadd.so` must exist side-by-side with :file:`hadoukend` for
it to be found.

As an example, to enable *AutoAdd*, you must copy the file from the *lib*
folder to the *bin* folder.

.. code:: bash

  $ cp lib/libautoadd.so bin/
