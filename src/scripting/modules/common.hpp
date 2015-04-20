#ifndef HADOUKEN_SCRIPTING_MODULES_COMMON_HPP
#define HADOUKEN_SCRIPTING_MODULES_COMMON_HPP

#include "../duktape.h"

#define DUK_READONLY_PROPERTY(ctx, index, name, func) \
    duk_push_string(ctx, #name); \
    duk_push_c_function(ctx, func, 0); \
    duk_def_prop(ctx, index, DUK_DEFPROP_HAVE_GETTER | DUK_DEFPROP_HAVE_ENUMERABLE | DUK_DEFPROP_ENUMERABLE);

namespace Hadouken
{
    namespace Scripting
    {
        namespace Modules
        {
            class Common
            {
            public:
                template<typename T>
                static T* getPointer(duk_context* ctx, const char* fieldName)
                {
                    duk_push_this(ctx);

                    T* res = 0;

                    if (duk_get_prop_string(ctx, -1, fieldName))
                    {
                        res = static_cast<T*>(duk_get_pointer(ctx, -1));
                        duk_pop(ctx);
                    }

                    duk_pop(ctx);
                    return res;
                }
            };
        }
    }
}

#endif
