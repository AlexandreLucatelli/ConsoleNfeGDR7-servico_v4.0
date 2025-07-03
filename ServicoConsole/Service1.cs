using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
namespace ServicoConsole
	{
	public partial class Service1 : ServiceBase
		{
		public Service1()
			{
			InitializeComponent();
			Console.Write("OK");
			Thread objThread = new Thread(LoopControl);
			objThread.Start();
			
			}
		private void LoopControl()
			{
			while (1 == 1)
				{
				loopEngine objLoop = new loopEngine();
				objLoop.Engine();
				Thread.Sleep(60000);
				}
			}
		protected override void OnStart(string[] args)
			{
			}

		protected override void OnStop()
			{
			}
		}
	}
