#ifndef HADOUKEN_SCRIPTING_SCRIPTINGSUBSYSTEM_HPP
#define HADOUKEN_SCRIPTING_SCRIPTINGSUBSYSTEM_HPP

#include <Hadouken/Config.hpp>
#include <Poco/Util/Application.h>
#include <Poco/Util/Subsystem.h>

#include <memory>
#include <mutex>
#include <string>
#include <thread>

using namespace Poco::Util;

namespace libtorrent
{
    class alert;
}

namespace Hadouken
{
    namespace Scripting
    {
        class ScriptingSubsystem : public Subsystem
        {
        public:
            HDKN_EXPORT ScriptingSubsystem();

            HDKN_EXPORT void emit(std::string name, libtorrent::alert* alert);

            HDKN_EXPORT std::string rpc(std::string request);

            HDKN_EXPORT std::string getScriptPath();

            HDKN_EXPORT std::string getScript();

        protected:
            void initialize(Application& app);

            void uninitialize();

            void tick();

            void read();

            const char* name() const;

        private:
            typedef void duk_context;
            
            static int requireNative(duk_context* ctx);

            bool isRunning_;
            Poco::Logger& logger_;
            duk_context* ctx_;
            std::mutex contextMutex_;
            std::thread ticker_;
            std::thread reader_;
        };
    }
}

#endif
