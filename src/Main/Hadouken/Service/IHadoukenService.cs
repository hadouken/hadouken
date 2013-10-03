using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Service
{
	public interface IHadoukenService
	{
		void Start(string[] args);
		void Stop();
	}
}
