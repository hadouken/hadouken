#include <hadouken/scripting/js_engine.hpp>

#include <v8.h>

using namespace hadouken::scripting;
using namespace v8;

void js_engine::load()
{
    V8::Initialize();
}
