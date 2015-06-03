#ifndef HADOUKEN_SCRIPTING_MODULES_BENCODINGMODULE_HPP
#define HADOUKEN_SCRIPTING_MODULES_BENCODINGMODULE_HPP

namespace Hadouken
{
    namespace Scripting
    {
        namespace Modules
        {
            class BEncodingModule
            {
            public:
                static int initialize(void* ctx);

            private:
                static int decode(void* ctx);
                static int encode(void* ctx);
            };
        }
    }
}

#endif