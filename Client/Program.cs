using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Client";
            
            Console.WriteLine("Press any key to connect to host:");

            ConsoleKeyInfo k;
            while (true)
            {
                k = Console.ReadKey();
                if (k.KeyChar == 'q' || k.KeyChar == 'Q')
                {
                    return;
                }

                //System.Threading.ThreadPool.QueueUserWorkItem(GetThreadProc);
                // System.Threading.ThreadPool.QueueUserWorkItem(setThreadProc);
                // System.Threading.ThreadPool.QueueUserWorkItem(putThreadProc);
                // System.Threading.ThreadPool.QueueUserWorkItem(dumpThreadProc);

                //Task.Run(GetThreadProc);
                GetThreadProc().ContinueWith(x => Console.WriteLine("Done"));
                Console.WriteLine("Press any key to repeat and q to quit...");
            }
        }

        static async Task GetThreadProc()
        {
            int threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Calling WCF Server from thread id...." + threadId);

            ServiceReference1.StockMarketClient proxy = new ServiceReference1.StockMarketClient();
            await proxy.getStockValueAsync("");

        }

        static void setThreadProc(Object stateInfo)
        {
            int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Calling WCF Server from thread id...." + threadID);

            ServiceReference1.StockMarketClient proxy = new ServiceReference1.StockMarketClient();


            string stockName = "threadID-" + threadID;

            for (int j = 0; j < 30; j++)
            {
                stockName = "threadID-" + threadID + "-" + j + "-";
                for (int i = 0; i < 100; i++)
                {
                    //proxy.BeginsetStockValue(stockName + i, null, null);

                    if (i % 10 == 0)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                System.Threading.Thread.Sleep(10000);
            }
        }

        static void putThreadProc(Object stateInfo)
        {
            int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Calling WCF Server from thread id...." + threadID);

            ServiceReference1.StockMarketClient proxy = new ServiceReference1.StockMarketClient();


            string stockName = "threadID-" + threadID;

            for (int j = 0; j < 30; j++)
            {
                stockName = "threadID-" + threadID + "-" + j + "-";
                for (int i = 0; i < 100; i++)
                {
                  //  proxy.BeginputStockValue(stockName + i, null, null);

                    if (i % 10 == 0)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                System.Threading.Thread.Sleep(10000);
            }
        }

        static void dumpThreadProc(Object stateInfo)
        {
            int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Calling WCF Server from thread id...." + threadID);

            ServiceReference1.StockMarketClient proxy = new ServiceReference1.StockMarketClient();


            string stockName = "threadID-" + threadID;

            for (int j = 0; j < 30; j++)
            {
                stockName = "threadID-" + threadID + "-" + j + "-";
                
              // proxy.BegindumpStockValue(stockName + j, null, null);

               // System.Threading.Thread.Sleep(10000);
            }
        }
    }
}
