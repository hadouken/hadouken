#ifndef HADOUKEN_EXTENSION_HPP
#define HADOUKEN_EXTENSION_HPP

namespace Hadouken
{
    namespace Extensions
    {
        class Extension
        {
        public:
            virtual ~Extension() {}
            
            virtual void load() = 0;

            virtual void unload() {}
        };
    }
}

#endif
