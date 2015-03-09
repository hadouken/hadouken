#ifndef AUTOADD_FOLDER_HPP
#define AUTOADD_FOLDER_HPP

#include <regex>
#include <string>

namespace AutoAdd
{
    struct Folder
    {
        Folder(std::string sourcePath, std::regex filePattern)
        {
            sourcePath_ = sourcePath;
            filePattern_ = filePattern;
        }

        std::string getSourcePath()
        {
            return sourcePath_;
        }

        std::regex getFilePattern()
        {
            return filePattern_;
        }

    private:
        std::string sourcePath_;
        std::regex filePattern_;
    };
}

#endif
