#ifndef HADOUKEN_SCRIPTING_MODULES_LOGGERMODULE_HPP
#define HADOUKEN_SCRIPTING_MODULES_LOGGERMODULE_HPP

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            class logger_module
            {
            public:
                static int initialize(void* ctx);

            private:
                static int get(void* ctx);

                static int log_trace(void* ctx);
                static int log_debug(void* ctx);
                static int log_info(void* ctx);
                static int log_warn(void* ctx);
                static int log_error(void* ctx);
            };
        }
    }
}

#endif