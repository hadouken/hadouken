#include "automoveextension.hpp"

#include <Hadouken/Extensions/Extension.hpp>
#include <Poco/ClassLibrary.h>

POCO_BEGIN_MANIFEST(Hadouken::Extensions::Extension)
    POCO_EXPORT_CLASS(AutoMove::AutoMoveExtension)
POCO_END_MANIFEST
