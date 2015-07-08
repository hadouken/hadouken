Building Hadouken on Ubuntu
===========================

Overview
--------

This will guide you through the process of building Hadouken on Ubuntu
using G++. *libtorrent* and *cpp-netlib* must be manually placed in the
:file:`deps/` directory.


Prerequisites
-------------

In order to successfully clone and build Hadouken you need the following
applications installed,

* :code:`build-essential` - provides g++ and other required build tools.
* :code:`cmake` - the build system Hadouken uses.
* :code:`git` - to get the latest Hadouken sources.
* :code:`libboost-system-dev` - required for Rasterbar-libtorrent.
* :code:`libssl-dev` - for HTTPS and SSL functionality in both Hadouken and
  Rasterbar-libtorrent.


Cloning the repository
----------------------

Clone the Hadouken GitHub repository at https://github.com/hadouken/hadouken.

.. code:: bash

   $ git clone https://github.com/hadouken/hadouken


Obtaining source dependencies
-----------------------------

The following source dependencies should be downloaded and extracted to the
:file:`deps/` directory.

* `libtorrent 1.0.5 <http://sourceforge.net/projects/libtorrent/files/libtorrent/libtorrent-rasterbar-1.0.5.tar.gz/download>`_
* `cpp-netlib 0.11.1 <http://storage.googleapis.com/cpp-netlib-downloads/0.11.1/cpp-netlib-0.11.1-final.zip>`_

The :file:`deps/` directory should look like this,

* deps/

  * libtorrent-rasterbar-1.0.5/
  * cpp-netlib-0.11.1/


Running the build
-----------------

By now you should have all you need to build Hadouken.

.. code:: bash

   $ mkdir cmake-build
   $ cd cmake-build/
   $ cmake ..
   $ make
