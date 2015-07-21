Building Hadouken on Ubuntu
===========================

Overview
--------

This will guide you through the process of building Hadouken on Ubuntu.
The depenencies *libtorrent* and *cpp-netlib* exists as Git submodules.


What you need
-------------

In order to successfully clone and build Hadouken you need the following
applications and libraries installed.

* :code:`g++-4.9`
* :code:`cmake`
* :code:`git`
* :code:`libssl-dev`
* :code:`libboost-1.58`

.. note:: Hadouken will not compile with a `GCC` version lower than 4.9 since
          that is the version which shipped with C++14 support.


Cloning the repository
----------------------

Clone `the Hadouken GitHub repository <https://github.com/hadouken/hadouken>`_.

.. code:: bash

   $ git clone https://github.com/hadouken/hadouken
   $ cd hadouken
   $ git submodule update --init


Running the build
-----------------

By now you should have all you need to build Hadouken.

.. code:: bash

   $ ./linux/build.sh
