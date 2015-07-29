Building Hadouken on Ubuntu
===========================

Overview
--------

This will guide you through the process of building Hadouken on Ubuntu 14.04.2
(Trusty Tahr). The instructions here will most probably work on older Ubuntu
versions (the Travis CI build for Hadouken runs on 12.04), as well as on Debian
and other Debian derivatives.


What you need
-------------

In order to successfully clone and build Hadouken you need the following
applications and libraries installed.

* :code:`cmake`
* :code:`git`
* :code:`libssl-dev`


Cloning the repository
----------------------

Clone `the Hadouken GitHub repository <https://github.com/hadouken/hadouken>`_.

.. code:: bash

   $ git clone https://github.com/hadouken/hadouken
   $ cd hadouken
   $ git submodule update --init


Running the build
-----------------

By now you should have all you need to build Hadouken. The repository has two
helper scripts which will download and compile Boost (1.58) and libtorrent
(1.0.5).

Boost and libtorrent are installed to :file:`%HOME%/boost/` and
:file:`%HOME%/libtorrent/`.

.. code:: bash

   $ ./linux/install-boost.sh
   $ ./linux/install-libtorrent.sh

Now, run the build.

.. code:: bash

   $ ./linux/build.sh

This should have produced both a Debian package file in the directory
:file:`linux/build` and a binary at :file:`linux/build/bin`.
