using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Configuration;
using System.IO;

namespace ConsoleNfeService
	{
	public partial class Service1 : ServiceBase
		{
		public Service1()
			{
			InitializeComponent();
			Engine();
			}
		public void Engine()
			{
			
			//objNFe.
			}
		protected override void OnStart(string[] args)
			{
				
			}

		protected override void OnStop()
			{
			}
		}
	}
