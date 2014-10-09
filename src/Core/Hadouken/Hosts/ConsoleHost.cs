using System;
using Hadouken.Core;

namespace Hadouken.Hosts
{
    public sealed class ConsoleHost
    {
        private readonly IHadoukenService _hadoukenService;

        public ConsoleHost(IHadoukenService hadoukenService)
        {
            if (hadoukenService == null) throw new ArgumentNullException("hadoukenService");
            _hadoukenService = hadoukenService;
        }

        public void Run(string[] args)
        {
            _hadoukenService.Load(args);

            Console.TreatControlCAsInput = true;

            while (true)
            {
                var key = Console.ReadKey(true);

                if ((key.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control && key.Key == ConsoleKey.C)
                {
                    break;
                }
            }

            _hadoukenService.Unload();

            Console.WriteLine("Hadouken has exited. Press any key to close.");
            Console.ReadKey();
        }
    }
}
