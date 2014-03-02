using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Service.Hosts
{
	public sealed class ServiceHost : ServiceBase
	{
		private readonly IHadoukenService _service;

		public ServiceHost(IHadoukenService service)
		{
			_service = service;
		}

		protected override void OnStart(string[] args)
		{
			// Start the service.
			_service.Start(args);

			// Call the base class method.
			base.OnStart(args);
		}

		protected override void OnStop()
		{
			// Start the service.
			_service.Stop();

			// Call the base class method.
			base.OnStop();
		}
	}
}
