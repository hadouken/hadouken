Building Hadouken on Ubuntu
===========================

Overview
--------

This will guide you through the process of building Hadouken on Ubuntu 14.04.2
(Trusty Tahr). The depenencies *libtorrent* and *cpp-netlib* exists as Git
submodules.


What you need
-------------

In order to successfully clone and build Hadouken you need the following
applications and libraries installed.

* :code:`cmake`
* :code:`git`
* :code:`libssl-dev`
* :code:`libboost-1.58`


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
