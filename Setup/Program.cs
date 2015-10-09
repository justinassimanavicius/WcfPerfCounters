using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
					ServiceModelTimeTaken.ServiceModelTimeTakenPerfmonCounters.DeleteCategory();
					ServiceModelTimeTaken.ServiceModelTimeTakenPerfmonCounters.CreateCategory();

					Console.WriteLine("ServiceModelTimeTaken perfmon counters installed.");
				}
				else if (String.Compare(args[0], "-u", StringComparison.OrdinalIgnoreCase) == 0)
				{
					ServiceModelTimeTaken.ServiceModelTimeTakenPerfmonCounters.DeleteCategory();

					Console.WriteLine("ServiceModelTimeTaken perfmon counters uninstalled.");
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