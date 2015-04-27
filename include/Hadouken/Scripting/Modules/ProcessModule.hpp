#ifndef HADOUKEN_SCRIPTING_MODULES_PROCESSMODULE_HPP
#define HADOUKEN_SCRIPTING_MODULES_PROCESSMODULE_HPP

namespace Hadouken
{
    namespace Scripting
    {
        namespace Modules
        {
            class ProcessModule
            {
            public:
                static int initialize(void* ctx);

            private:
                static int launch(void* ctx);
            };
        }
    }
}

#endif