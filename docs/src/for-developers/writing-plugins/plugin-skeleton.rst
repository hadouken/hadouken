Basic plugin skeleton
=====================

Overview
--------

The most basic plugin for Hadouken does nothing. It is a simple :file:`.js` file
that exports a load function.


The plugin directory
--------------------

Depending on whether Hadouken is installed or running portable, the JavaScript
root varies.

 - If Hadouken is installed, plugins are (by default) loaded from
   :file:`%PROGRAMDATA%/Hadouken/js/plugins`.
 - If Hadouken is running portable, plugins are (by default) loaded
   from :file:`js/plugins` which is relative to where the :file:`hadouken.exe`
   resides.


Basic plugin
------------

.. code:: javascript

   exports.load = function() {};
