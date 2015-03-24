#include "../duktape.h"

namespace JsEngine
{
    namespace Modules
    {
        class FileSystem
        {
        public:
            static duk_ret_t init(duk_context* ctx);

        private:
            static duk_ret_t getFiles(duk_context* ctx);
            static duk_ret_t readFile(duk_context* ctx);
            static duk_ret_t removeFile(duk_context* ctx);

            static const duk_function_list_entry functions_[];
        };
    }
}
