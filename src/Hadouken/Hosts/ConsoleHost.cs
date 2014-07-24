using System;
using Hadouken.Core;

namespace Hadouken.Hosts
{
    public sealed class ConsoleHost
    {
        private readonly IService _service;

        public ConsoleHost(IService service)
        {
            if (service == null) throw new ArgumentNullException("service");
            _service = service;
        }

        public void Run(string[] args)
        {
            _service.Load(args);

            Console.TreatControlCAsInput = true;

            while (true)
            {
                var key = Console.ReadKey(true);

                if ((key.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control && key.Key == ConsoleKey.C)
                {
                    break;
                }
            }

            _service.Unload();
        }
    }
}
