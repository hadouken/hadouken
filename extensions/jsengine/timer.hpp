#ifndef JSENGINE_TIMER_HPP
#define JSENGINE_TIMER_HPP

#include "duktape.h"

#include <Poco/Clock.h>

namespace JsEngine
{
    class Timer
    {
    public:
        static void init(duk_context* ctx);

        static void run(duk_context* ctx, Poco::Clock& timerClock, uint32_t* currentTO);
    };
}

#endif
