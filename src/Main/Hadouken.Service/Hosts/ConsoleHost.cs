using System;
using System.Threading;

namespace Hadouken.Service.Hosts
{
	public sealed class ConsoleHost
	{
		private readonly IHadoukenService _service;

		public ConsoleHost(IHadoukenService service)
		{
			_service = service;
		}

		public void Run()
		{
            // Treat Ctrl+C as regular input to prevent it from killing child processes.
		    Console.TreatControlCAsInput = true;

			var args = Environment.GetCommandLineArgs();

			// Start the service.
			_service.Start(args);

			Console.BackgroundColor = ConsoleColor.Blue;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine();
			Console.Write(new string(' ', Console.WindowWidth));
            Console.Write(" Press CTRL+C to stop service".PadRight(Console.WindowWidth));
            Console.Write(new string(' ', Console.WindowWidth));
			Console.WriteLine();
			Console.ResetColor();

			// Wait for a key.
		    while (true)
		    {
		        var key = Console.ReadKey(true);

		        if ((key.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control && key.Key == ConsoleKey.C)
		        {
		            break;
		        }
		    }

			// Stop the service.
			_service.Stop();

		    Console.WriteLine();
            Console.Write(" Press any key to close window");
		    Console.Read();
		}
	}
}
