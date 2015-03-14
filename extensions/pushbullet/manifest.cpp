#include "pushbulletextension.hpp"

#include <Hadouken/Extensions/Extension.hpp>
#include <Poco/ClassLibrary.h>

POCO_BEGIN_MANIFEST(Hadouken::Extensions::Extension)
    POCO_EXPORT_CLASS(Pushbullet::PushbulletExtension)
POCO_END_MANIFEST
