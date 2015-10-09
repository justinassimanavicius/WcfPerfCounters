using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Setup
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Count() == 0)
                {
                    Console.WriteLine("ServiceModelTimeTaken perfmon counters....");
                    Console.WriteLine("Valid options are -i to install and -u to uninstall");
                    return;
                }

                if (string.Compare(args[0], "-i", true) == 0)
                {
                    ServiceModelTimeTaken.ServiceModelTimeTakenPerfmonCounters.DeleteCategory();
                    ServiceModelTimeTaken.ServiceModelTimeTakenPerfmonCounters.CreateCategory();

                    Console.WriteLine("ServiceModelTimeTaken perfmon counters installed.");
                }
                else if (string.Compare(args[0], "-u", true) == 0)
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
