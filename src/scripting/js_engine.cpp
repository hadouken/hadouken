#include <hadouken/scripting/js_engine.hpp>

#include <include/v8.h>
#include <include/libplatform/libplatform.h>

using namespace hadouken::scripting;
using namespace v8;

void js_engine::load()
{
    V8::Initialize();
}
