#ifndef HADOUKEN_SCRIPTING_MODULES_CONFIGMODULE_HPP
#define HADOUKEN_SCRIPTING_MODULES_CONFIGMODULE_HPP

namespace Hadouken
{
    namespace Scripting
    {
        namespace Modules
        {
            class ConfigModule
            {
            public:
                static int initialize(void* ctx);

            private:
                static int getBoolean(void* ctx);
                static int getNumber(void* ctx);
                static int getString(void* ctx);
                static int has(void* ctx);
                static int setString(void* ctx);
            };
        }
    }
}

#endif
