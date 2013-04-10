using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.Http.Mvc;

namespace Hadouken.Common.Http.ControllerFactory
{
    public class DefaultControllerFactory : IControllerFactory
    {
        private readonly IEnvironment _environment;
        private readonly IController[] _controllers;

        public DefaultControllerFactory(IEnvironment environment, IController[] controllers)
        {
            _environment = environment;
            _controllers = controllers;
        }

        public void Execute(IHttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}
