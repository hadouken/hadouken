#ifndef HADOUKEN_SCRIPTING_MODULES_HTTPMODULE_HPP
#define HADOUKEN_SCRIPTING_MODULES_HTTPMODULE_HPP

namespace Hadouken
{
    namespace Scripting
    {
        namespace Modules
        {
            class HttpModule
            {
            public:
                static int initialize(void* ctx);

            private:
                static int post(void* ctx);
            };
        }
    }
}

#endif