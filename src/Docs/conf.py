# sphinx config

import os, sys
sys.path.append(os.path.dirname(__file__))

from datetime import date
from pysemver import SemVer

f = open(os.path.join(os.path.dirname(__file__), "../../VERSION"))
vf = f.read()
v = SemVer(vf)
f.close()

extensions = []

# templates, relative to this file
#templates_path = ["_templates"]

# exclude patterns
exclude_patterns = ["_build"]

source_suffix = ".rst"
master_doc = "index"

project = "Hadouken"
copyright = date.today().strftime("%Y") + ", " + project

version = str(v.major) + "." + str(v.minor)
release = vf

pygments_style = "sphinx"

# html theming
html_theme = "default"

htmlhelp_basename = "hdkndoc"