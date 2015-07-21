Building Hadouken on Windows
============================

Overview
--------

This will guide you through the process of building Hadouken on Windows
using MSVC 12 (Visual Studio 2013). Most of the dependencies are pre-built
and will be pulled in during the build process, however *libtorrent* and
*cpp-netlib* are placed in-source via Git submodules.


Prerequisites
-------------

In order to successfully clone and build Hadouken you need the following
applications installed,

* MSVC 12.0 (Visual Studio 2013)
* Git
* CMake (>= 2.8)


Cloning the repository
----------------------

Clone `the Hadouken GitHub repository <https://github.com/hadouken/hadouken>`_
and initialize the submodules.

.. code:: powershell

   C:\Code> git clone https://github.com/hadouken/hadouken
   C:\Code> cd hadouken
   C:\Code\hadouken> git submodule update --init


Running the build
-----------------

By now you should have all you need to build Hadouken. Other dependencies such
as OpenSSL and Boost will be automatically downloaded in the build process.

.. code:: powershell

   C:\Code\hadouken> .\win32\build.ps1
