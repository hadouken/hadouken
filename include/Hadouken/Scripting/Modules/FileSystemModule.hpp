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
                static int combine(void* ctx);
                static int deleteFile(void* ctx);
                static int directoryExists(void* ctx);
                static int fileExists(void* ctx);
                static int getFiles(void* ctx);
                static int isRelative(void* ctx);
                static int readBuffer(void* ctx);
                static int readText(void* ctx);
                static int writeBuffer(void* ctx);
                static int writeText(void* ctx);
            };
        }
    }
}

#endif