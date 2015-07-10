Building Hadouken on Ubuntu
===========================

Overview
--------

This will guide you through the process of building Hadouken on Ubuntu.
*libtorrent* and *cpp-netlib* must be placed in the :file:`deps/` directory.
If you run the :file:`./bootstrap.sh` script, this will be taken care of.


What you need
-------------

In order to successfully clone and build Hadouken you need the following
applications and libraries installed.

* :code:`g++-4.9`
* :code:`cmake`
* :code:`git`
* :code:`libssl-dev`
* :code:`libboost-1.58`

.. note:: Hadouken will not compile with a `g++` version lower than 4.9 since
          that is the version which shipped with C++14 support.


Cloning the repository
----------------------

Clone the Hadouken GitHub repository at https://github.com/hadouken/hadouken.

.. code:: bash

   $ git clone https://github.com/hadouken/hadouken


Bootstrapping
-----------------------------

The repository contains a bootstrap script which will prepare your
environment.

.. code:: bash

   $ ./linux/bootstrap.sh


Running the build
-----------------

By now you should have all you need to build Hadouken.

.. code:: bash

   $ ./linux/build.sh
