#ifndef HADOUKEN_SCRIPTING_MODULES_HTTPMODULE_HPP
#define HADOUKEN_SCRIPTING_MODULES_HTTPMODULE_HPP

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            class http_module
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