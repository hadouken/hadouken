The Hadouken SDK
================

This SDK contains everything you need to get started with plugin development
for Hadouken. For the complete documentation and beginners tutorial - see
http://wiki.hdkn.net

---

1. Inherit the abstract class Hadouken.Fx.Plugin and override
the abstract Load function. There is also a virtual method called
Unload you can override to clean up after yourself.

public class MyPlugin : Hadouken.Fx.Plugin
{
    public override void Load() { }

    public override void Unload() { }
}

---

2. Add a manifest.json file which describes your plugin and specifies
dependencies. It must be called `manifest.json` live in the project root. It
looks like this,

{
  "manifest_version": 1,

  "id": "<id of your plugin>",
  "version": "<semver>"
}

What we are doing here is just making the `config` plugin a dependency, as
well as requiring the correct version of it. If you do not have any
dependencies on other plugins you can leave out the dependencies array.

The `manifest.json` file should have a build action of "Content" and copy to
output directory should be "Copy always" or "Copy if newer".

Hadouken will fail to load your plugin if this file does not exist in your
plugin directory root or at the root of your plugin package file.

---

3. Set the correct startup project. To be able to debug your plugin as easily
as possible, you need to set the start action of your project to
"Start external program" and also set the path to the Hadouken.Service.exe
which resides in `packages/Hadouken.SDK.x.x.x/tools/hdkn/` relative to your
solution file.

The command line arguments must be set to

--no-http-bootstrap --plugin "<full path to your output folder>"

Finally, set working directory to the same folder as the Hadouken.Service.exe
lives in.

---

4. Set a breakpoint in your Load function and press F5. Hadouken should
start, do its thing, and then load your plugin as well as hitting your
breakpoint.

IMPORTANT! To manage Hadouken, the default endpoint is http://localhost:7890/manage
so point your browser of choice there and explore.

Now go do magic. Happy coding!