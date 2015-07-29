
Installing Hadouken on Ubuntu
=============================

Overview
--------

This will guide you through the installation of Hadouken on Ubuntu (Trusty
Tahr, 14.04.2).

Hadouken runs command line application and you can either run it in `screen`,
with `start-stop-daemon` or as a regular application from the terminal.


Getting the installer
`````````````````````

As of v5.2 we provide :file:`.deb` packages. These are the recommended way of
installing Hadouken on Ubuntu. Download the latest package from
`our GitHub release feed <https://github.com/hadouken/hadouken/releases>`_.


Installing
----------

Download the latest :file:`.deb` package to your home directory and either
double click it or install it from a terminal.

.. code:: bash

   $ sudo dpkg -i hadouken-5.2.0.deb


Running Hadouken
----------------

Hadouken installs a configuration template file to
:file:`/etc/hadouken/hadouken.json.template` which you should edit and adjust
to your needs. You *will* need to change the values for
`bittorrent.defaultSavePath` and `bittorrent.statePath`. Otherwise, you can use
the default settings.

Save the template to :file:`/etc/hadouken/hadouken.json` and and start Hadouken
with the following command,

.. code:: bash

   $ hadouken --config="/etc/hadouken/hadouken.json"

Now, point a browser to :file:`http://localhost:7070/gui/index.html` and get
started!
