#ifndef HADOUKEN_SCRIPTING_MODULES_FILESYSTEMMODULE_HPP
#define HADOUKEN_SCRIPTING_MODULES_FILESYSTEMMODULE_HPP

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            class file_system_module
            {
            public:
                static int initialize(void* ctx);

            private:
                static int combine(void* ctx);
                static int create_directories(void* ctx);
                static int delete_file(void* ctx);
                static int directory_exists(void* ctx);
                static int file_exists(void* ctx);
                static int get_files(void* ctx);
                static int is_relative(void* ctx);
                static int make_absolute(void* ctx);
                static int read_buffer(void* ctx);
                static int read_text(void* ctx);
                static int rename(void* ctx);
                static int space(void* ctx);
                static int write_buffer(void* ctx);
                static int write_text(void* ctx);
            };
        }
    }
}

#endif