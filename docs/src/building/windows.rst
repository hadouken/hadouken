Building Hadouken on Windows
============================

Overview
--------

This will guide you through the process of building Hadouken on Windows
using MSVC 12 (Visual Studio 2013). Most of the dependencies are pre-built
and will be pulled in during the build process, however *libtorrent* and
*cpp-netlib* must be manually placed in the :file:`deps/` directory.


Prerequisites
-------------

In order to successfully clone and build Hadouken you need the following
applications installed,

* MSVC 12.0 (Visual Studio 2013)
* Git
* CMake (>= 2.8)


Cloning the repository
----------------------

Clone the Hadouken GitHub repository at https://github.com/hadouken/hadouken.

.. code:: powershell

   C:\Code> git clone https://github.com/hadouken/hadouken


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

By now you should have all you need to build Hadouken. Other dependencies such
as OpenSSL and Boost will be automatically downloaded in the build process.

.. code:: powershell

   C:\Code\hadouken> .\win32\build.ps1
