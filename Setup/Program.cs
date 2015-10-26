using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using WcfPerformanceCounters;

namespace Setup
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			try
			{

				if (!args.Any())
				{
					Console.WriteLine("ServiceModelTimeTaken perfmon counters....");
					Console.WriteLine("Valid options are -i to install and -u to uninstall");
					return;
				}

				if (String.Compare(args[0], "-i", StringComparison.OrdinalIgnoreCase) == 0)
				{
					PerfmonCounters.DeleteCategory();
					PerfmonCounters.CreateCategory();

					Console.WriteLine("ServiceModelTimeTaken perfmon counters installed.");
				}
				else if (String.Compare(args[0], "-u", StringComparison.OrdinalIgnoreCase) == 0)
				{
					PerfmonCounters.DeleteCategory();

					Console.WriteLine("ServiceModelTimeTaken perfmon counters uninstalled.");
				}
				else if (String.Compare(args[0], "-p", StringComparison.OrdinalIgnoreCase) == 0)
				{
					int i = int.MaxValue - 10;
					while (! Console.KeyAvailable)
					{
						i += 1;
						var perfConter = PerfmonCounters.GetCounter("test");
						Thread.Sleep(500);
						perfConter.Executing.RawValue = i;// DateTime.Now.Second % 19;
						perfConter.Hits.RawValue = DateTime.Now.Second % 4;
						perfConter.TimeTaken.RawValue = DateTime.Now.Second % 7;

						Console.WriteLine("ServiceModelTimeTaken perfmon counters updated. "+ i);
					}
				}
				else
				{
					Console.WriteLine("ServiceModelTimeTaken perfmon counters....");
					Console.WriteLine("Valid options are -i to install and -u to uninstall");
				}
			}
			catch (Exception ex)
			{
				Console.Write(ex.ToString());
			}
		}
	}
}