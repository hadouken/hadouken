#ifndef HADOUKEN_SCRIPTING_MODULES_FILESYSTEMMODULE_HPP
#define HADOUKEN_SCRIPTING_MODULES_FILESYSTEMMODULE_HPP

namespace Hadouken
{
    namespace Scripting
    {
        namespace Modules
        {
            class FileSystemModule
            {
            public:
                static int initialize(void* ctx);

            private:
                static int deleteFile(void* ctx);
                static int getFiles(void* ctx);
                static int readFile(void* ctx);
            };
        }
    }
}

#endif