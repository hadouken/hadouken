/******************************************************************************
* Copyright AllSeen Alliance. All rights reserved.
*
*    Permission to use, copy, modify, and/or distribute this software for any
*    purpose with or without fee is hereby granted, provided that the above
*    copyright notice and this permission notice appear in all copies.
*
*    THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
*    WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
*    MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
*    ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
*    WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
*    ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
*    OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
******************************************************************************/

#include "timer.hpp"

#define TRUE  1
#define FALSE 0
#undef  min

#include <algorithm>

using namespace JsEngine;

typedef struct _HDKN_TIMER {
    uint8_t  isInterval; // TRUE for an interval timer
    uint32_t id;         // Id for the timer composed from index + salt value
    uint32_t interval;   // Scheduling interval in milliseconds - not used for oneshot timers
    uint32_t countDown;  // Count down time
} HDKN_TIMER;

#define NUM_TIMERS 2   // number of timers to allocate initially
#define ADD_TIMERS 2   // numer of timers to add if we need to expand the table
#define MAX_TIMERS 256 // maximum number of timers

#define GET_TIMER_INDEX(id)   ((id) >> 24)
#define SALT_TIMER_ID(index)  ((index << 24) | (++timerSalt & 0xFFFFFF))

// Salt for uniquefying timer identifiers
static uint32_t timerSalt;

// Current deadline - zero means we don't know the deadline
static uint32_t deadline = 0;

// The same function is used to register interval and one-shot timers
static int RegisterTimer(duk_context* ctx, uint32_t ms, uint8_t isInterval)
{
    HDKN_TIMER* timers;
    size_t numTimers;
    size_t timerEntry;

    if (!duk_is_callable(ctx, 0))
    {
        duk_error(ctx, DUK_ERR_TYPE_ERROR, "First argument must be a function");
    }
    
    // Get the global state management objects
    duk_push_global_stash(ctx);
    duk_get_prop_string(ctx, -1, "timerFuncs");
    duk_get_prop_string(ctx, -2, "timerState");

    timers = static_cast<HDKN_TIMER*>(duk_get_buffer(ctx, -1, &numTimers));
    numTimers = numTimers / sizeof(HDKN_TIMER);
    
    // Find first free slot in the timer table expanding the table if necessary
    for (timerEntry = 0; timerEntry <= numTimers; ++timerEntry)
    {
        if (timerEntry == numTimers)
        {
            numTimers += ADD_TIMERS;
        
            if (numTimers > MAX_TIMERS)
            {
                duk_error(ctx, DUK_ERR_ALLOC_ERROR, "Too many timers");
            }

            timers = static_cast<HDKN_TIMER*>(duk_resize_buffer(ctx, -1, numTimers * sizeof(HDKN_TIMER)));
            
            if (!timers)
            {
                duk_error(ctx, DUK_ERR_ALLOC_ERROR, "Could not allocate timer");
            }
        }

        // Zero indicates an unused timer entry
        if (timers[timerEntry].id == 0)
        {
            break;
        }
    }

    // Pop the buffer off the stack
    duk_pop(ctx);
    
    // Push the callable function onto the stack and set in array
    duk_dup(ctx, 0);
    duk_put_prop_index(ctx, -2, timerEntry);
    
    // Pop the callable, the timerFuncs array and the global stash
    duk_pop_3(ctx);
    
    // Push the salted timer index, this is what we will return from this call
    // Zero is reserved to indicate unused entries.
    do
    {
        timers[timerEntry].id = SALT_TIMER_ID(timerEntry);
    } while (!timers[timerEntry].id);
    
    duk_push_int(ctx, timers[timerEntry].id);
    
    // Set the interval and initialize the countDown timer
    timers[timerEntry].isInterval = isInterval;
    timers[timerEntry].interval = ms;
    timers[timerEntry].countDown = ms;
    
    // We need to recompute the deadline
    deadline = 0;

    return 1;
}

static HDKN_TIMER* GetTimer(duk_context* ctx, uint8_t isInterval)
{
    HDKN_TIMER* timers;
    size_t numTimers;
    uint32_t timerId = (uint32_t)duk_require_int(ctx, 0);
    uint32_t timerEntry = GET_TIMER_INDEX(timerId);

    duk_push_global_stash(ctx);
    duk_get_prop_string(ctx, -1, "timerState");
    timers = static_cast<HDKN_TIMER*>(duk_get_buffer(ctx, -1, &numTimers));
    numTimers = numTimers / sizeof(HDKN_TIMER);

    // Check timer exists
    if ((timerEntry > numTimers) || (timerId != timers[timerEntry].id))
    {
        duk_error(ctx, DUK_ERR_RANGE_ERROR, "No such timer");
    }
    
    // Check timer has the expected type
    if (isInterval != timers[timerEntry].isInterval)
    {
        duk_error(ctx, DUK_ERR_TYPE_ERROR, "Timer has wrong type for this operation");
    }
    
    duk_pop_3(ctx);
    
    return &timers[timerEntry];
}

static int ClearTimer(duk_context* ctx, uint8_t isInterval)
{
    HDKN_TIMER* timer = GetTimer(ctx, isInterval);
    uint32_t timerEntry = GET_TIMER_INDEX(timer->id);

    // Clear timer
    timer->id = 0;

    // Delete the timer entry
    duk_push_global_stash(ctx);
    duk_get_prop_string(ctx, -1, "timerFuncs");
    duk_del_prop_index(ctx, -1, timerEntry);
    duk_pop_2(ctx);
    
    // We need to recompute the deadline
    deadline = 0;

    return 0;
}

static int NativeSetInterval(duk_context* ctx)
{
    uint32_t ms = duk_require_uint(ctx, 1);

    if (ms == 0) {
        duk_error(ctx, DUK_ERR_RANGE_ERROR, "Interval must be > 0");
    }

    return RegisterTimer(ctx, ms, TRUE);
}

static int NativeSetTimeout(duk_context* ctx)
{
    uint32_t ms = duk_require_uint(ctx, 1);
    return RegisterTimer(ctx, ms, FALSE);
}

static int ResetTimer(duk_context* ctx, uint8_t isInterval)
{
    uint32_t ms = duk_require_uint(ctx, 1);
    HDKN_TIMER* timer = GetTimer(ctx, isInterval);

    if (isInterval && (ms == 0))
    {
        duk_error(ctx, DUK_ERR_RANGE_ERROR, "Interval must be > 0");
    }
    
    // Reset the timer properties
    timer->interval = ms;
    timer->countDown = ms;
    
    // We need to recompute the deadline
    deadline = 0;
    
    // Push the timer id, this is what we will return from this call
    duk_push_int(ctx, timer->id);

    return 1;
}

static int NativeResetInterval(duk_context* ctx)
{
    return ResetTimer(ctx, TRUE);
}

static int NativeResetTimeout(duk_context* ctx)
{
    return ResetTimer(ctx, FALSE);
}

static int NativeClearInterval(duk_context* ctx)
{
    return ClearTimer(ctx, TRUE);
}

static int NativeClearTimeout(duk_context* ctx)
{
    return ClearTimer(ctx, FALSE);
}

static const duk_function_list_entry timer_native_functions[] = {
    { "setInterval", NativeSetInterval, 2 },
    { "clearInterval", NativeClearInterval, 1 },
    { "resetInterval", NativeResetInterval, 2 },
    { "setTimeout", NativeSetTimeout, 2 },
    { "clearTimeout", NativeClearTimeout, 1 },
    { "resetTimeout", NativeResetTimeout, 2 },
    { NULL }
};

void Timer::init(duk_context* ctx)
{
    /*
    * Timer state is managed via two global stash properties, "timerFuncs" is an array that holds
    * references to the timer callback functions, "timerState" is a memory blob that holds an array
    * of C structs that provide information about the active timers.
    */
    duk_push_global_stash(ctx);
    duk_push_array(ctx);
    duk_put_prop_string(ctx, -2, "timerFuncs");
    duk_push_dynamic_buffer(ctx, NUM_TIMERS * sizeof(HDKN_TIMER));
    duk_put_prop_string(ctx, -2, "timerState");
    duk_pop(ctx);

    // Register interval and timeout functions
    duk_push_global_object(ctx);
    duk_put_function_list(ctx, -1, timer_native_functions);
    duk_pop(ctx);

    deadline = 0;
}

void Timer::run(duk_context* ctx, Poco::Clock& timerClock, uint32_t* currentTO)
{
    HDKN_TIMER* timers;
    size_t numTimers;
    size_t timerEntry;
    uint32_t elapsed;

    // Time elapsed since this function was last called
    Poco::Clock currentTime;
    elapsed = (currentTime - timerClock) / 1000;
    timerClock += elapsed * 1000;
    
    // Get the global state management objects
    duk_push_global_stash(ctx);
    duk_get_prop_string(ctx, -1, "timerState");
    timers = static_cast<HDKN_TIMER*>(duk_get_buffer(ctx, -1, &numTimers));
    numTimers = numTimers / sizeof(HDKN_TIMER);
    duk_get_prop_string(ctx, -2, "timerFuncs");
    
    // Iterate over the timers and call functions at or past the deadline
    for (timerEntry = 0; timerEntry < numTimers; ++timerEntry)
    {
        HDKN_TIMER* timer = &timers[timerEntry];

        if (timer->id != 0)
        {            
            if (timer->countDown <= elapsed)
            {
                // Get timer function on the stack
                duk_get_prop_index(ctx, -1, timerEntry);
                
                // Update an interval timer or delete a one-shot timeout timer.
                if (timer->isInterval)
                {
                    uint32_t delta = elapsed - deadline;
                    
                    if (delta > timer->interval)
                    {
                        printf("Unable to meet interval schedule\n");
                        delta = 0;
                    }
                    
                    timer->countDown = timer->interval - delta;
                }
                else
                {
                    timer->id = 0;
                    duk_del_prop_index(ctx, -2, timerEntry);
                }
                
                // Call the timer function
                if (duk_pcall(ctx, 0) != DUK_EXEC_SUCCESS)
                {
                    printf("%s\n", duk_to_string(ctx, -1));
                }
                
                duk_pop(ctx); // return value
            }
            else
            {
                timer->countDown -= elapsed;
            }
        }
    }

    // Pop timer funcs array
    duk_pop(ctx);
    
    // Reload the timer state and calculate the new deadline.
    timers = static_cast<HDKN_TIMER*>(duk_get_buffer(ctx, -1, &numTimers));
    numTimers = numTimers / sizeof(HDKN_TIMER);
    deadline = 0xFFFFFFFF;
    
    for (timerEntry = 0; timerEntry < numTimers; ++timerEntry)
    {
        if (timers[timerEntry].id != 0)
        {
            deadline = std::min(timers[timerEntry].countDown, deadline);
        }
    }

    duk_pop_2(ctx);
    *currentTO = deadline;
}
