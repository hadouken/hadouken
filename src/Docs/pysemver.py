# -*- coding: utf-8 -*-
"""
pysemver provides handy utilities to work with versions that satisfy
Semantic Versioning Specification requirements. Allows validate, parse,
compare, increment version numbers, etc.

Homepage and documentation: https://github.com/antonmoiseev/pysemver

Copyright (c) 2011 Anton Moiseev

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
"""

__version__ = '0.5.0'

import re


_semver_regex = re.compile(r"^([0-9]+)" # major
                         + r"\.([0-9]+)" # minor
                         + r"\.([0-9]+)" # patch
                         + r"(\-[0-9A-Za-z]+(\.[0-9A-Za-z]+)*)?" # pre-release
                         + r"(\+[0-9A-Za-z]+(\.[0-9A-Za-z]+)*)?$") # build


class SemVer:

    def __init__(self, version=None):
        self._major = self._minor = self._patch = 0
        self._build = self._prerelease = None

        if version:
            match = _semver_regex.match(version)
            if match:
                self._major = int(match.group(1))
                self._minor = int(match.group(2))
                self._patch = int(match.group(3))

                if match.group(4):
                    self._prerelease = match.group(4).lstrip('-')

                if match.group(6):
                    self._build = match.group(6).lstrip('+')
            else:
                raise ValueError('{0} is not valid SemVer string'.format(version))

    @property
    def major(self):
        return self._major

    @property
    def minor(self):
        return self._minor

    @property
    def patch(self):
        return self._patch

    @property
    def prerelease(self):
        return self._prerelease

    @property
    def build (self):
        return self._build

    def is_prerelease(self):
        return self._prerelease != None

    def is_build(self):
        return self._build != None

    @staticmethod
    def is_valid(version):
        return True if _semver_regex.match(version) else False