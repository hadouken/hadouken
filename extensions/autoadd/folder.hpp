#ifndef AUTOADD_FOLDER_HPP
#define AUTOADD_FOLDER_HPP

#include <regex>
#include <string>
#include <vector>

namespace AutoAdd
{
    struct Folder
    {
        std::string path;
        std::regex pattern;
        std::string savePath;
        std::vector<std::string> tags;
    };
}

#endif
