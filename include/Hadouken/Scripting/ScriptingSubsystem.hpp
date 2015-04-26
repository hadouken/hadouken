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

namespace Hadouken
{
    namespace Scripting
    {
        class Event;

        class ScriptingSubsystem : public Subsystem
        {
        public:
            HDKN_EXPORT ScriptingSubsystem();

            HDKN_EXPORT void emit(std::string eventName, std::unique_ptr<Event> data);

            HDKN_EXPORT std::string rpc(std::string request);

            HDKN_EXPORT std::string getScriptPath();

            HDKN_EXPORT std::string getScript();

        protected:
            void initialize(Application& app);

            void uninitialize();

            void tick();

            const char* name() const;

        private:
            typedef void duk_context;
            
            static int requireNative(duk_context* ctx);

            bool isRunning_;
            Poco::Logger& logger_;
            duk_context* ctx_;
            std::mutex contextMutex_;
            std::thread ticker_;
        };
    }
}

#endif
