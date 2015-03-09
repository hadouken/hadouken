#ifndef HADOUKEN_EXTENSION_HPP
#define HADOUKEN_EXTENSION_HPP

#include <Poco/Util/AbstractConfiguration.h>

namespace Hadouken
{
    namespace Extensions
    {
        class Extension
        {
        public:
            virtual ~Extension() {}
            
            virtual void load(Poco::Util::AbstractConfiguration& config) = 0;

            virtual void unload() {}
        };
    }
}

#endif
