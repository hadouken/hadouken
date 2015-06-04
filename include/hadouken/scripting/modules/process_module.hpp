#ifndef HADOUKEN_SCRIPTING_MODULES_PROCESSMODULE_HPP
#define HADOUKEN_SCRIPTING_MODULES_PROCESSMODULE_HPP

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            class process_module
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