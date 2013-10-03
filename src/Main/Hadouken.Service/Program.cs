﻿using System;
using System.ServiceProcess;
using Autofac;
using System.Threading;
using Hadouken.Service.Hosts;

namespace Hadouken.Service
{
	public class Program
	{
		#region Application Entry Point
		[STAThread]
		public static void Main()
		{
			new Program().Run();
		}
		#endregion

		public void Run()
		{
			// Create the Hadouken service.
			var service = new Bootstrapper().Build();

			if (Environment.UserInteractive)
			{
				// Run as a console application.
				new ConsoleHost(service).Run();
			}
			else
			{
				// Create a Windows service.
				var host = new ServiceHost(service);
				ServiceBase.Run(new ServiceBase[] { host });
			}
		}
	}
}
