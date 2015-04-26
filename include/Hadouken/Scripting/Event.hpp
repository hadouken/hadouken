#ifndef HADOUKEN_SCRIPTING_EVENT_HPP
#define HADOUKEN_SCRIPTING_EVENT_HPP

namespace Hadouken
{
    namespace Scripting
    {
        class Event
        {
        public:
            virtual void push(void* ctx) = 0;
        };
    }
}

#endif
