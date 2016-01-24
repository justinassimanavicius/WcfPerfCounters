using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Host
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Host";

            //
            //  WCF config are in App.Config
            //
            ServiceHost host = new ServiceHost(typeof(StockMarket));

            host.Open();

            Console.WriteLine("WCF Service is running...");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            host.Close();
        }
    }

    [ServiceContract]
    public interface IStockMarket
    {
        [OperationContract]
        double getStockValue(string strSymbol);
        [OperationContract]
        double setStockValue(string strSymbol);
        [OperationContract]
        double putStockValue(string strSymbol);
        [OperationContract]
        double dumpStockValue(string strSymbol);
    }
    public class StockMarket : IStockMarket
    {
        static Random r = new Random((int)DateTime.Now.Ticks);

        public double dumpStockValue(string strSymbol)
        {
            string output = string.Format("DUMP : client requested stock value for symbol = {0}", strSymbol);
            System.Console.WriteLine(output);

            var next = 1;// r.Next(25, 100);
            System.Threading.Thread.Sleep(next * 1000);
            return 100.0;
        }

        public double getStockValue(string strSymbol)
        {
            string output = string.Format("GET : client requested stock value for symbol = {0}", strSymbol);
            System.Console.WriteLine(output);

            System.Threading.Thread.Sleep(r.Next(1, 10) * 100);
            return 100.0;
        }

        public double setStockValue(string strSymbol)
        {
            string output = string.Format("SET : client requested stock value for symbol = {0}", strSymbol);
            System.Console.WriteLine(output);

            System.Threading.Thread.Sleep(r.Next(1, 25) * 1000);
            return 100.0;
        }

        public double putStockValue(string strSymbol)
        {
            string output = string.Format("PUT : client requested stock value for symbol = {0}", strSymbol);
            System.Console.WriteLine(output);

            System.Threading.Thread.Sleep(r.Next(1, 25) * 1000 );
            return 100.0;
        }

    }

}
