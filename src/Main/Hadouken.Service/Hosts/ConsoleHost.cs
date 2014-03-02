using System;
using System.Threading;

namespace Hadouken.Service.Hosts
{
	public sealed class ConsoleHost
	{
		private readonly IHadoukenService _service;
		private readonly ManualResetEvent _stopEvent;

		public ConsoleHost(IHadoukenService service)
		{
			_service = service;
			_stopEvent = new ManualResetEvent(false);
		}

		public void Run()
		{
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
			Console.CancelKeyPress += this.OnCancelKeyPress;
			_stopEvent.WaitOne();

			// Stop the service.
			_service.Stop();

		    Console.WriteLine();
            Console.Write(" Press any key to close window");
		    Console.Read();
		}

		private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			// Prevent Windows from shutting the console down.
			e.Cancel = true;

			// Signal that CTRL+C has been pressed.
			_stopEvent.Set();
		}
	}
}
