#include <iostream>

#include <boost/asio/io_service.hpp>
#include <boost/filesystem.hpp>
#include <boost/log/trivial.hpp>
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/json_parser.hpp>
#include <boost/python.hpp>

#include <hadouken/plugin_manager.hpp>

using namespace hadouken;
namespace py = boost::python;

std::string parse_python_exception();

class plugin
{
public:
    virtual void load() = 0;

    virtual void unload() {}
};

class plugin_wrapper : public plugin
{
public:
    plugin_wrapper(PyObject* p)
        : self(p)
    {
    }

    void load()
    {
        py::call_method<void>(self, "load");
    }

private:
    PyObject* self;
};

BOOST_PYTHON_MODULE(hadouken)
{
    py::class_<plugin, boost::noncopyable, boost::shared_ptr<plugin_wrapper>>("Plugin")
        .def("load", &plugin_wrapper::load)
        ;
}

plugin_manager::plugin_manager(hadouken::service_locator& service_locator)
    : service_locator_(service_locator)
{
    // Initialize Python interpreter but do not
    // subscribe to any signals.
    PyImport_AppendInittab("hadouken", &inithadouken);
    Py_InitializeEx(0);
}

plugin_manager::~plugin_manager()
{
}

void plugin_manager::load()
{
    py::object main_module = py::import("__main__");
    py::object global = main_module.attr("__dict__");

    try
    {
        py::exec("import sys\nsys.path.append('C:/temp')\n", global, global);
        py::object o = py::import("auto_move");
        py::object f = o.attr(py::str("AutoMove"))();
        
        plugin& pl = py::extract<plugin&>(f);
        pl.load();
        

        //py::object result = py::exec_file("C:/temp/hadouken.py", global, global);
        //py::object f = py::getattr(result, "foo");

        plugins_.insert(std::make_pair("hdkn", PyEval_SaveThread()));

        BOOST_LOG_TRIVIAL(debug) << "executed script";
    }
    catch (py::error_already_set const &)
    {
        BOOST_LOG_TRIVIAL(error) << parse_python_exception();
    }
}

void plugin_manager::unload()
{
}

// Parses the value of the active python exception
// NOTE SHOULD NOT BE CALLED IF NO EXCEPTION
std::string parse_python_exception()
{
    PyObject *type_ptr = NULL, *value_ptr = NULL, *traceback_ptr = NULL;
    // Fetch the exception info from the Python C API
    PyErr_Fetch(&type_ptr, &value_ptr, &traceback_ptr);

    // Fallback error
    std::string ret("Unfetchable Python error");
    // If the fetch got a type pointer, parse the type into the exception string
    if (type_ptr != NULL){
        py::handle<> h_type(type_ptr);
        py::str type_pstr(h_type);
        // Extract the string from the boost::python object
        py::extract<std::string> e_type_pstr(type_pstr);
        // If a valid string extraction is available, use it 
        //  otherwise use fallback
        if (e_type_pstr.check())
            ret = e_type_pstr();
        else
            ret = "Unknown exception type";
    }
    // Do the same for the exception value (the stringification of the exception)
    if (value_ptr != NULL){
        py::handle<> h_val(value_ptr);
        py::str a(h_val);
        py::extract<std::string> returned(a);
        if (returned.check())
            ret += ": " + returned();
        else
            ret += std::string(": Unparseable Python error: ");
    }
    // Parse lines from the traceback using the Python traceback module
    if (traceback_ptr != NULL){
        py::handle<> h_tb(traceback_ptr);
        // Load the traceback module and the format_tb function
        py::object tb(py::import("traceback"));
        py::object fmt_tb(tb.attr("format_tb"));
        // Call format_tb to get a list of traceback strings
        py::object tb_list(fmt_tb(h_tb));
        // Join the traceback strings into a single string
        py::object tb_str(py::str("\n").join(tb_list));
        // Extract the string, check the extraction, and fallback in necessary
        py::extract<std::string> returned(tb_str);
        if (returned.check())
            ret += ": " + returned();
        else
            ret += std::string(": Unparseable Python traceback");
    }
    return ret;
}