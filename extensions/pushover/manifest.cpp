#include "pushoverextension.hpp"

#include <Hadouken/Extensions/Extension.hpp>
#include <Poco/ClassLibrary.h>

POCO_BEGIN_MANIFEST(Hadouken::Extensions::Extension)
    POCO_EXPORT_CLASS(Pushover::PushoverExtension)
POCO_END_MANIFEST
