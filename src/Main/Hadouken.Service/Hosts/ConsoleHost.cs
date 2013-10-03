﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
			Console.Write(new string(' ', 80));
			Console.Write(" Press CTRL+C to stop service".PadRight(80));
			Console.Write(new string(' ', 80));
			Console.WriteLine();
			Console.ResetColor();

			// Wait for a key.
			Console.CancelKeyPress += this.OnCancelKeyPress;
			_stopEvent.WaitOne();

			// Stop the service.
			_service.Stop();
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
